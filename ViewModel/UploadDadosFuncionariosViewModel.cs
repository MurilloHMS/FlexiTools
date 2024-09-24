using ClosedXML.Excel;
using FlexiTools.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;

namespace FlexiTools.ViewModel
{
    public class UploadDadosFuncionariosViewModel : ViewModelBase
    {

        private static readonly string funcionariosFile = "funcionarios.json";
        private static readonly string categoriasFile = "Categorias.json";

        private ObservableCollection<Cartao> _cartoes;
        private ICollectionView _cartoesGroupedView;
        private string texto;

        public ObservableCollection<Cartao> Cartoes
        {
            get => _cartoes;
            set
            {
                _cartoes = value;
                OnPropertyChanged(nameof(Cartoes));
                UpdateGroupedData();
            }
        }

        public ICollectionView CartoesGroupedView
        {
            get => _cartoesGroupedView;
            private set
            {
                _cartoesGroupedView = value;
                OnPropertyChanged(nameof(CartoesGroupedView));
            }
        }

        public ICommand ImportExcelComamnd { get; }
        public ICommand SaveCommand { get; }

        public UploadDadosFuncionariosViewModel()
        {
            Cartoes = new ObservableCollection<Cartao>();
            ImportExcelComamnd = new RelayCommand(async () => await ImportExcelData());
            SaveCommand = new RelayCommand(async () => await SaveData(), CanSaveData);
        }

        private async Task ImportExcelData()
        {
            var dados = await ObterDadosDasPlanilhasAsync();
            Cartoes.Clear();
            foreach (var cartao in dados)
            {
                Cartoes.Add(cartao);
            }
            UpdateGroupedData();
        }

        private void UpdateGroupedData()
        {
            var groupedData = Cartoes
                .GroupBy(c => new { c.Nome })
                .Select(g => new
                {
                    g.Key.Nome,
                    Categorias = g
                        .GroupBy(c => c.Categoria)
                        .Select(cg => new
                        {
                            Categoria = cg.Key,
                            ValorTotal = cg.Sum(c => c.Valor)
                        })
                })
                .ToList();

            CartoesGroupedView = new CollectionViewSource { Source = groupedData }.View;
            CartoesGroupedView.Refresh();
        }

        private async Task SaveData()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Arquivos Excel (*.xlsx)|*.xlsx",
                Title = "Escolha onde salvar o arquivo"
            };

            if (dialog.ShowDialog() == true)
            {
                string caminhoArquivo = dialog.FileName;

                try
                {
                    await SalvarDados(caminhoArquivo, Cartoes);
                    MessageBox.Show("Dados salvos com sucesso.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar dados: {ex.Message}");
                }
            }
        }

        private bool CanSaveData()
        {
            return Cartoes.Count > 0;
        }

        public static async Task<IEnumerable<Cartao>> ObterDadosDasPlanilhasAsync()
        {
            var dados = new List<Cartao>();

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Todos os Arquivos (*.*)|*.*",
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
