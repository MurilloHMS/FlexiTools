using Microsoft.Win32;
using ClosedXML.Excel;
using System.Windows;
using System.IO;
using System.Collections.Immutable;

namespace FlexiTools.MVVM.Model
{
    public class Cartao
    {
        public string Nome { get; set; }
        public DateTime? Data { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string Categoria { get; set; }
        public string Cliente { get; set; }

        private static readonly string funcionariosFile = "funcionarios.json";
        private static readonly string categoriasFile = "Categorias.json";

        public static async Task GerarPlanilhasPorFuncionarioAsync(IProgress<string> progress)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Todos os Arquivos (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() != true)
            {
                MessageBox.Show("Nenhum arquivo selecionado.");
                return;
            }

            string filePath = ofd.FileName;
            var dadosFuncionarios = new Dictionary<string, List<(DateTime Data, string Descricao, decimal Valor)>>();

            try
            {
                progress?.Report("Obtendo dados dos Funcionários...");

                var funcionarios = await Funcionario.GetFuncionarios(funcionariosFile);

                progress?.Report("Lendo dados do arquivo...");

                await Task.Run(() =>
                {
                    using (XLWorkbook workbook = new XLWorkbook(filePath))
                    {
                        var sheet = workbook.Worksheet(1);

                        string currentFuncionario = null;
                        bool coleta = false;

                        foreach (var row in sheet.RowsUsed())
                        {
                            string rowData = string.Join("\t", row.CellsUsed().Select(cell => cell.GetValue<string>()));

                            if (string.IsNullOrWhiteSpace(rowData)) continue;

                            if (funcionarios.Any(identifier => rowData.Contains(identifier.Nome)))
                            {
                                currentFuncionario = rowData;
                                if (!dadosFuncionarios.ContainsKey(currentFuncionario))
                                {
                                    dadosFuncionarios[currentFuncionario] = new List<(DateTime Data, string Descricao, decimal Valor)>();
                                }
                            }

                            if (rowData.Contains("data"))
                            {
                                coleta = true;
                            }
                            else if (rowData.Contains("Total de lançamentos nacionais") || rowData.Contains("Lançamentos nacionais"))
                            {
                                coleta = false;
                            }

                            if (coleta && currentFuncionario != null)
                            {
                                var columns = rowData.Split('\t');
                                if (columns.Length >= 3)
                                {
                                    if (DateTime.TryParse(columns[0], out DateTime data))
                                    {
                                        if (decimal.TryParse(columns[2], out decimal valor))
                                        {
                                            dadosFuncionarios[currentFuncionario].Add((data, columns[1], valor));
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                string savePath = Path.Combine(Path.GetDirectoryName(filePath), "Funcionarios");
                Directory.CreateDirectory(savePath);

                progress?.Report("Gerando planilhas...");

                await Task.Run(() =>
                {
                    foreach (var funcionario in dadosFuncionarios)
                    {
                        string texto = funcionario.Key;
                        string nomeArquivo = $"{texto.Substring(0, texto.IndexOf('-'))}.xlsx";
                        string caminhoArquivo = Path.Combine(savePath, nomeArquivo);

                        using (var newWorkbook = new XLWorkbook())
                        {
                            var worksheet = newWorkbook.AddWorksheet("Dados");

                            worksheet.Cell(1, 1).Value = "Data";
                            worksheet.Cell(1, 2).Value = "Descrição";
                            worksheet.Cell(1, 3).Value = "Valor";
                            worksheet.Cell(1, 4).Value = "Categoria Gasto";
                            worksheet.Cell(1, 5).Value = "Cliente";

                            int row = 2;
                            foreach (var (data, descricao, valor) in funcionario.Value)
                            {
                                worksheet.Cell(row, 1).Value = data;
                                worksheet.Cell(row, 2).Value = descricao;
                                worksheet.Cell(row, 3).Value = valor;

                                var categorias = Categorias.GetCategoriasAsync(categoriasFile);
                                var valir = categorias.Result.Select(x => x.Nome);
                                var validCategorias = $"\"{string.Join(",", valir)}\"";
                                var validation = worksheet.Cell(row, 4).CreateDataValidation();
                                validation.List(validCategorias, true);
                                validation.ErrorTitle = "Categorias Bloqueadas";
                                validation.ErrorStyle = XLErrorStyle.Information;
                                validation.ErrorMessage = "'Só é possível incluir as categorias pré selecionadas";
                                row++;

                            }

                            var validaton = worksheet.Range("A1:C10000").CreateDataValidation();
                            validaton.Custom($"\">=0\"");
                            validaton.ErrorTitle = "Edição Bloqueada";
                            validaton.ErrorStyle = XLErrorStyle.Information;
                            validaton.ErrorMessage = "Célula Bloqueada!";



                            newWorkbook.SaveAs(caminhoArquivo);
                        }

                        progress?.Report($"Planilha gerada: {nomeArquivo}");
                    }
                });

                progress?.Report("Planilhas geradas com sucesso.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar o arquivo Excel: {ex.Message}");
            }
        }

        public static async Task<IEnumerable<Cartao>> ObterDadosDasPlanilhasAsync()
        {
            var dados = new List<Cartao>();

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Multiselect = true
            };

            if (ofd.ShowDialog() != true)
            {
                MessageBox.Show("Nenhum arquivo selecionado.");
                return Enumerable.Empty<Cartao>();
            }

            string[] files = ofd.FileNames;

            foreach (string file in files)
            {
                string nome = Path.GetFileName(file);
                string formatedName = nome.Substring(0, nome.IndexOf("."));

                await Task.Run(() =>
                {
                    using (XLWorkbook wb = new XLWorkbook(file))
                    {
                        var planilha = wb.Worksheet(1);
                        var fileData = planilha.RowsUsed()
                                        .Skip(1)
                                        .Select(row => new Cartao
                                        {
                                            Data = row.Cell(1).TryGetValue<DateTime>(out var data) ? data : null,
                                            Descricao = row.Cell(2).TryGetValue<string>(out var descricao) ? descricao : null,
                                            Valor = row.Cell(3).TryGetValue<decimal>(out var valor) ? valor : 0m,
                                            Categoria = row.Cell(4).TryGetValue<string>(out var categoria) ? categoria : null,
                                            Cliente = row.Cell(5).TryGetValue<string>(out var cliente) ? cliente : null,
                                            Nome = row.Cell(6).TryGetValue<string>(out var nome) && !string.IsNullOrWhiteSpace(nome) ? nome : formatedName

                                        }).ToList();

                        lock (dados)
                        {
                            dados.AddRange(fileData);
                        }
                    }
                });
            }

            return dados;

        }

        public static async Task SalvarDados(string caminhoArquivo, IEnumerable<Cartao> dados)
        {
            await Task.Run(() =>
            {
                using (var workbook = File.Exists(caminhoArquivo) ? new XLWorkbook(caminhoArquivo) : new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Count > 0 ? workbook.Worksheet(1) : workbook.Worksheets.Add("Dados");

                    var ultimaLinha = worksheet.CellsUsed().Any() ? worksheet.CellsUsed().Last().Address.RowNumber : 0;

                    int novaLinha = ultimaLinha + 1;

                    foreach (var row in dados)
                    {
                        worksheet.Cell(novaLinha, 1).Value = row.Data;
                        worksheet.Cell(novaLinha, 2).Value = row.Descricao;
                        worksheet.Cell(novaLinha, 3).Value = row.Valor;
                        worksheet.Cell(novaLinha, 4).Value = row.Categoria;
                        worksheet.Cell(novaLinha, 5).Value = row.Cliente;
                        worksheet.Cell(novaLinha, 6).Value = row.Nome;
                        novaLinha++;
                    }

                    workbook.SaveAs(caminhoArquivo);
                }
            });
        }


    }
}
