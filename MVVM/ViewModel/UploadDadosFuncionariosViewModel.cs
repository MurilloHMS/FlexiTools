using FlexiTools.MVVM.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FlexiTools.MVVM.ViewModel
{
    public class UploadDadosFuncionariosViewModel : ViewModelBase
    {
        private ObservableCollection<Cartao> _cartoes;
        private ICollectionView _cartoesGroupedView;

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
            var dados = await Cartao.ObterDadosDasPlanilhasAsync();
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
                    Nome = g.Key.Nome,
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
