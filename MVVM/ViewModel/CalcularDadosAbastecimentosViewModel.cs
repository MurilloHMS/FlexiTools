using FlexiTools.MVVM.Model;
using Microsoft.Win32;
using OfficeOpenXml.Drawing.Chart.ChartEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FlexiTools.MVVM.ViewModel
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
            AbrirArquivo = new RelayCommand(async () => await ImportExcelData());
            SalvarArquivo = new RelayCommand(async () => await SaveExcelData(), canSave);

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
                var dados = await Abastecimentos.GetAbastecimentosAsync();
                var funcionarios = await Funcionario.GetFuncionarios("funcionarios.json");
                Abastecimento.Clear();


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
                    }
                    abs.Validacao = abs.MediaKm < abs.ConsumoUrbanoGasolina ? "Consumo Médio menor que o informado pelo Fabricante" : "";
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
            return plate.Replace("-","").ToUpper();
        }

        private async Task SaveExcelData()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Excel Files|*.xlsx"
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            await Abastecimentos.SaveDataAsync(ofd.FileName, Abastecimento);

            
        }

        private bool canSave()
        {
            return Abastecimento.Count > 0;
        }
    }

}
