using FlexiTools.MVVM.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FlexiTools.MVVM.ViewModel
{
    public class UploadDadosFuncionariosViewModel : ViewModelBase
    {
        private ObservableCollection<Cartao> _cartoes;

        public ObservableCollection<Cartao> Cartoes
        {
            get => _cartoes;
            set
            {
                _cartoes = value;
                OnPropertyChanged(nameof(Cartoes));
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
            var dados = await Cartao.ObterDadosDasPlanilhasAsync();
            Cartoes.Clear();
            foreach(var cartao in dados)
            {
                Cartoes.Add(cartao);
            }
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
                    await Cartao.SalvarDados(caminhoArquivo, Cartoes);
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
    }
}
