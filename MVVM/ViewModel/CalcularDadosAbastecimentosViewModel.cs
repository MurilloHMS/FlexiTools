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

        public ICommand AbrirArquivo { get; }
        public ICommand SalvarArquivo { get; }

        public CalcularDadosAbastecimentosViewModel()
        {

            Abastecimento = new ObservableCollection<Abastecimentos>();
            AbrirArquivo = new RelayCommand(async () => await ImportExcelData());
            SalvarArquivo = new RelayCommand(async () => await SaveExcelData());
        }

        private async Task ImportExcelData()
        {
            var dados = await Abastecimentos.GetAbastecimentosAsync();
            Abastecimento.Clear();
            foreach(var abs in dados)
            {
                Abastecimento.Add(abs);
            }
        }

        private async Task SaveExcelData()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            await Abastecimentos.SaveDataAsync(ofd.FileName, Abastecimento);

            
        }
    }
}
