using ClosedXML.Excel;
using FlexiTools.Model;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace FlexiTools.ViewModel
{
    internal class DadosCartoesViewModel : ViewModelBase
    {

        private static readonly string funcionariosFile = "funcionarios.json";
        private static readonly string categoriasFile = "Categorias.json";

        private string _conteudo;
        public string Conteudo
        {
            get => _conteudo;
            set
            {
                _conteudo = value;
                OnPropertyChanged(nameof(Conteudo));
            }
        }

        public ICommand GerarDadosCommand { get; }

        public DadosCartoesViewModel()
        {
            GerarDadosCommand = new RelayCommand(async () => await GerarDadosAsync());
        }

        private async Task GerarDadosAsync()
        {
            StringBuilder sb = new StringBuilder();

            var progress = new Progress<string>(message =>
            {
                sb.AppendLine(message);
                Conteudo = sb.ToString();
            });

            try
            {
                await GerarPlanilhasPorFuncionarioAsync(progress);
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Erro: {ex.Message}");
                Conteudo = sb.ToString();
            }
        }

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
                    using XLWorkbook workbook = new(filePath);
                    var sheet = workbook.Worksheet(1);

                    string currentFuncionario = string.Empty;
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
    }
}
