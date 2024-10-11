using ClosedXML.Excel;
using FlexiTools.Model;
using FlexiTools.Model.Repositories;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace FlexiTools.ViewModel
{
    internal class CalcularDadosAbastecimentosViewModel : ViewModelBase
    {
        private ObservableCollection<Abastecimentos> _abastecimentos;

        public ObservableCollection<Abastecimentos> Abastecimento
        {
            get => _abastecimentos;
            set
            {
                _abastecimentos = value;
                OnPropertyChanged(nameof(Abastecimentos));
            }
        }

        private readonly Dictionary<string, Vehicle> _vehicles;

        public ICommand AbrirArquivo { get; }
        public ICommand SalvarArquivo { get; }

        public CalcularDadosAbastecimentosViewModel()
        {

            Abastecimento = new ObservableCollection<Abastecimentos>();
            AbrirArquivo = new RelayCommand(async _ => await ImportExcelData());
            SalvarArquivo = new RelayCommand(async _ => await SaveExcelData(), _ => CanSave());

            _vehicles = new Dictionary<string, Vehicle>
            {
                { "FHH7I86", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "GJY9755", new Vehicle { Name = "NOVA SAVEIRO TL MBVS", Brand = "VOLKSWAGEN", UrbanAlcoholConsumption = 7.3, RoadAlcoholConsuption = 8.5, UrbanGasolineConsumption = 10.7, RoadGasolineConsuption = 12.3 } },
                { "RNK7B98", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "SIZ2J86", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "GIL0962", new Vehicle { Name = "NOVO GOL TL MCV", Brand = "VOLKSWAGEN", UrbanAlcoholConsumption = 8.9, RoadAlcoholConsuption = 10.4, UrbanGasolineConsumption = 13.1, RoadGasolineConsuption = 14.9 } },
                { "SIG9C92", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "CUG7H68", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "SST6A48", new Vehicle { Name = "KWID", Brand = "RENAUT", UrbanAlcoholConsumption = 10.3, RoadAlcoholConsuption = 10.8, UrbanGasolineConsumption = 14.9, RoadGasolineConsuption = 15.6 } },
                { "SHS3G04", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "FFQ9J27", new Vehicle { Name = "ONIX SD", Brand = "CHEVROLET", UrbanAlcoholConsumption = 8.6, RoadAlcoholConsuption = 10.9, UrbanGasolineConsumption = 12.0, RoadGasolineConsuption = 15.0 } },
                { "FZH2C11", new Vehicle { Name = "POLO", Brand = "VOLKSWAGEN", UrbanAlcoholConsumption = 9.3, RoadAlcoholConsuption = 10.5, UrbanGasolineConsumption = 13.5, RoadGasolineConsuption = 15.0 } },
                { "SIG9C93", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "MTZ3D69", new Vehicle { Name = "FIT", Brand = "HONDA", UrbanAlcoholConsumption = 7.6, RoadAlcoholConsuption = 8.7, UrbanGasolineConsumption = 11.4, RoadGasolineConsuption = 13.2 } },
                { "ETB5D61", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "DZV4286", new Vehicle { Name = "PALIO", Brand = "FIAT", UrbanAlcoholConsumption = 8.7, RoadAlcoholConsuption = 10.4, UrbanGasolineConsumption = 12.2, RoadGasolineConsuption = 15.3 } },
                { "KWW8892", new Vehicle { Name = "COBALT", Brand = "CHEVROLET", UrbanAlcoholConsumption = 7.2, RoadAlcoholConsuption = 9.9, UrbanGasolineConsumption = 9.4, RoadGasolineConsuption = 12.9 } },
                { "SHU1C35", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "BWV0I24", new Vehicle { Name = "ARGO", Brand = "FIAT", UrbanAlcoholConsumption = 9.9, RoadAlcoholConsuption = 10.7, UrbanGasolineConsumption = 14.2, RoadGasolineConsuption = 15.1 } },
                { "RUN8C05", new Vehicle { Name = "PRISMA", Brand = "CHEVROLET", UrbanAlcoholConsumption = 8.1, RoadAlcoholConsuption = 10.2, UrbanGasolineConsumption = 11.9, RoadGasolineConsuption = 14.7 } },
                { "SUC1D60", new Vehicle { Name = "POLO", Brand = "VOLKSWAGEN", UrbanAlcoholConsumption = 9.3, RoadAlcoholConsuption = 10.5, UrbanGasolineConsumption = 13.5, RoadGasolineConsuption = 15.0 } },
                { "SIS0E68", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } },
                { "GHK9J60", new Vehicle { Name = "GOL 1.0 MC4", Brand = "VOLKSWAGEN", UrbanAlcoholConsumption = 9.1, RoadAlcoholConsuption = 10.1, UrbanGasolineConsumption = 13.4, RoadGasolineConsuption = 14.4 } },
                { "ECU3A67", new Vehicle { Name = "ARGO", Brand = "FIAT", UrbanAlcoholConsumption = 9.9, RoadAlcoholConsuption = 10.7, UrbanGasolineConsumption = 14.2, RoadGasolineConsuption = 15.1 } },
                { "SJG1C06", new Vehicle { Name = "MOBI LIKE", Brand = "FIAT", UrbanAlcoholConsumption = 8.8, RoadAlcoholConsuption = 9.2, UrbanGasolineConsumption = 12.7, RoadGasolineConsuption = 13.3 } }
            };
        }

        private async Task ImportExcelData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                var dados = await GetAbastecimentosAsync();
                var funcionarios = await FuncionarioRepository.GetFuncionarios("funcionarios.json");
                Abastecimento.Clear();

                string[] mes = ["Jan","Fev","Mar","Abr","Mai","Jun","Jul","Ago","Set","Out","Nov","Dez"];

                foreach (var abs in dados)
                {
                    if (_vehicles.TryGetValue(NormalizePlate(abs.PlacaDoVeiculo), out var vehicle))
                    {
                        abs.ModeloDoVeiculo = vehicle.Name;
                        abs.FabricanteVeiculo = vehicle.Brand;
                        abs.ConsumoUrbanoAlcool = vehicle.UrbanAlcoholConsumption;
                        abs.ConsumoRodoviárioAlcool = vehicle.RoadAlcoholConsuption;
                        abs.ConsumoUrbanoGasolina = vehicle.UrbanGasolineConsumption;
                        abs.ConsumoRodoviárioGasolina = vehicle.RoadGasolineConsuption;
                    }
                    var funcionario = funcionarios.FirstOrDefault(x => x != null && x.Hash != null && x.Hash.Equals(abs.NomeDoMotorista));
                    if (funcionario != null)
                    {
                        abs.NomeDoMotorista = funcionario.Nome;
                        abs.SetorDoMotorista = funcionario.Departamento;
                        abs.Gerente = funcionario.Gerente ?? null;
                        abs.Email = funcionario.Email ?? null;
                    }
                    abs.Validacao = abs.MediaKm < abs.ConsumoUrbanoGasolina ? "Consumo Médio menor que o informado pelo Fabricante" : "";
                    abs.Data = DateTime.TryParse(abs.DataAbastecimento, out var datas) ? new DateTime(datas.Year, datas.Month, 01) : null;

                    Abastecimento.Add(abs);
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }

        private static string NormalizePlate(string plate)
        {
            return plate.Replace("-", "").ToUpper();
        }

        private async Task SaveExcelData()
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Excel Files|*.xlsx"
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            await SaveDataAsync(ofd.FileName, Abastecimento);


        }

        private bool CanSave()
        {
            return Abastecimento.Count > 0;
        }

        public static async Task<IEnumerable<Abastecimentos>> GetAbastecimentosAsync()
        {
            List<Abastecimentos> abastecimentos = [];

            OpenFileDialog ofd = new()
            {
                Multiselect = false,
                Title = "Selecione a planilha de abastecimento",
                Filter = "Excel Files|*.xlsx"
            };

            if (ofd.ShowDialog() != true)
            {
                MessageBox.Show("Nenhum arquivo selecionado", "Seleção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return [];
            }

            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook wb = new(ofd.FileName);                    
                    var planilha = wb.Worksheet(1);
                    var filedata = planilha.RowsUsed().Skip(1).Select(row => new Abastecimentos
                    {
                        DataAbastecimento = row.Cell(6).TryGetValue<string>(out var dataAbastecimento) ? dataAbastecimento : null,
                        StatusEmissaoNFe = row.Cell(7).TryGetValue<string>(out var statusEmissaoNFe) ? statusEmissaoNFe : null,
                        CNPJ = row.Cell(8).TryGetValue<string>(out var CNPJ) ? CNPJ : null,
                        RazaoSocial = row.Cell(9).TryGetValue<string>(out var RazaoSocial) ? RazaoSocial : null,
                        NomeFantasia = row.Cell(10).TryGetValue<string>(out var nomeFantasia) ? nomeFantasia : null,
                        CNPJEstabelecimento = row.Cell(11).TryGetValue<string>(out var cnpjEstabelecimento) ? cnpjEstabelecimento : null,
                        RazaoSocialEstabelecimento = row.Cell(12).TryGetValue<string>(out var razaoSocialEstabelecimento) ? razaoSocialEstabelecimento : null,
                        NomeFantasiaEstabelecimetno = row.Cell(13).TryGetValue<string>(out var nomeFantasiaEstabelecimento) ? nomeFantasiaEstabelecimento : null,
                        EnderecoEstabelecimento = row.Cell(14).TryGetValue<string>(out var enderecoEstabelecimento) ? enderecoEstabelecimento : null,
                        CidadeEstabelecimento = row.Cell(15).TryGetValue<string>(out var cidadeEstabelecimento) ? cidadeEstabelecimento : null,
                        UfEstabelecimento = row.Cell(16).TryGetValue<string>(out var ufEstabelecimento) ? ufEstabelecimento : null,
                        PlacaDoVeiculo = row.Cell(17).TryGetValue<string>(out var placaDoVeiculo) ? placaDoVeiculo : null,
                        NomeDoMotorista = row.Cell(21).TryGetValue<string>(out var nomeDoMotorista) ? nomeDoMotorista : null,
                        LatitudeElongitudeDispositivo = row.Cell(23).TryGetValue<string>(out var longitudeLatitudeDispositivo) ? longitudeLatitudeDispositivo : null,
                        latitudeElogitudePosto = row.Cell(24).TryGetValue<string>(out var longitudeLatitudePosto) ? longitudeLatitudePosto : null,
                        HodometroAtual = row.Cell(26).TryGetValue<decimal>(out var hodometroAtual) ? hodometroAtual : null,
                        HodometroAnterior = row.Cell(27).TryGetValue<decimal>(out var hodometroAnterior) ? hodometroAnterior : null,
                        DiferencaHodometro = row.Cell(28).TryGetValue<decimal>(out var diferencaHodometro) ? diferencaHodometro : null,
                        MediaKm = row.Cell(29).TryGetValue<double>(out var mediaKM) ? mediaKM : null,
                        MeioDePagamento = row.Cell(33).TryGetValue<string>(out var meioDePagamento) ? meioDePagamento : null,
                        Produto = row.Cell(40).TryGetValue<string>(out var produto) ? produto : null,
                        Litros = row.Cell(41).TryGetValue<decimal>(out var litros) ? litros : null,
                        VlrLitro = row.Cell(42).TryGetValue<decimal>(out var vlrLitro) ? vlrLitro : null,
                        VlrTotalProdutos = row.Cell(43).TryGetValue<decimal>(out var vlrTotalProdutos) ? vlrTotalProdutos : null,
                        VlrTotalValidado = vlrLitro * litros

                    }).ToList();

                    lock (abastecimentos)
                    {
                        abastecimentos.AddRange(filedata);
                    }
                    
                });
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ocorreu um erro ao abrir o arquivo: {ex.Message}", "IOException", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro inesperado: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return abastecimentos;
        }

        public static async Task SaveDataAsync(string path, IEnumerable<Abastecimentos> data)
        {
            try
            {
                await Task.Run(() =>
                {
                    using var workbook = File.Exists(path) ? new XLWorkbook(path) : new XLWorkbook();
                    var worksheet = workbook.Worksheets.Count > 1 ? workbook.Worksheet(2) : workbook.Worksheets.Add("Dados");

                    var lastLine = worksheet.CellsUsed().Any() ? worksheet.CellsUsed().Last().Address.RowNumber : 0;

                    int newLine = lastLine + 1;

                    foreach (var row in data)
                    {
                        worksheet.Cell(newLine, 1).Value = row.DataAbastecimento;
                        worksheet.Cell(newLine, 2).Value = row.Data;
                        worksheet.Cell(newLine, 3).Value = row.StatusEmissaoNFe;
                        worksheet.Cell(newLine, 4).Value = row.CNPJ;
                        worksheet.Cell(newLine, 5).Value = row.RazaoSocial;
                        worksheet.Cell(newLine, 6).Value = row.NomeFantasia;
                        worksheet.Cell(newLine, 7).Value = row.CNPJEstabelecimento;
                        worksheet.Cell(newLine, 8).Value = row.RazaoSocialEstabelecimento;
                        worksheet.Cell(newLine, 9).Value = row.NomeFantasiaEstabelecimetno;
                        worksheet.Cell(newLine, 10).Value = row.EnderecoEstabelecimento;
                        worksheet.Cell(newLine, 11).Value = row.CidadeEstabelecimento;
                        worksheet.Cell(newLine, 12).Value = row.UfEstabelecimento;
                        worksheet.Cell(newLine, 13).Value = row.PlacaDoVeiculo;
                        worksheet.Cell(newLine, 14).Value = row.ModeloDoVeiculo;
                        worksheet.Cell(newLine, 15).Value = row.FabricanteVeiculo;
                        worksheet.Cell(newLine, 16).Value = row.ConsumoUrbanoAlcool;
                        worksheet.Cell(newLine, 17).Value = row.ConsumoRodoviárioAlcool;
                        worksheet.Cell(newLine, 18).Value = row.ConsumoUrbanoGasolina;
                        worksheet.Cell(newLine, 19).Value = row.ConsumoRodoviárioGasolina;
                        worksheet.Cell(newLine, 20).Value = row.NomeDoMotorista;
                        worksheet.Cell(newLine, 21).Value = row.SetorDoMotorista;
                        worksheet.Cell(newLine, 22).Value = row.LatitudeElongitudeDispositivo;
                        worksheet.Cell(newLine, 23).Value = row.latitudeElogitudePosto;
                        worksheet.Cell(newLine, 24).Value = row.HodometroAtual;
                        worksheet.Cell(newLine, 25).Value = row.HodometroAnterior;
                        worksheet.Cell(newLine, 26).Value = row.DiferencaHodometro;
                        worksheet.Cell(newLine, 27).Value = row.MediaKm;
                        worksheet.Cell(newLine, 28).Value = row.MeioDePagamento;
                        worksheet.Cell(newLine, 29).Value = row.Produto;
                        worksheet.Cell(newLine, 30).Value = row.Litros;
                        worksheet.Cell(newLine, 31).Value = row.VlrLitro;
                        worksheet.Cell(newLine, 32).Value = row.VlrTotalProdutos;
                        worksheet.Cell(newLine, 33).Value = row.VlrTotalValidado;
                        worksheet.Cell(newLine, 34).Value = row.Validacao;
                        worksheet.Cell(newLine, 35).Value = row.Gerente;
                        worksheet.Cell(newLine, 36).Value = row.Email;
                        newLine++;
                    }

                    workbook.SaveAs(path);
                    MessageBox.Show("Dados Salvos com Sucesso!", "Dados Salvos", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ocorreu um erro ao salvar o arquivo: {ex.Message}", "IOException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro inesperado: {ex.Message}", "Error Handle", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }

}
