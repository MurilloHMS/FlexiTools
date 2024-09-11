using FlexiTools.MVVM.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FlexiTools.MVVM.ViewModel
{
    public class ConfiguracoesViewModel : ViewModelBase
    {
        private readonly string funcionariosFile = "funcionarios.json";
        private readonly string categoriasFile = "Categorias.json";

        private ObservableCollection<Funcionario> _funcionarios;
        private ObservableCollection<Categorias> _categorias;


        public ObservableCollection<Funcionario> Funcionarios
        {
            get => _funcionarios;
            set
            {
                _funcionarios = value;
                OnPropertyChanged(nameof(Funcionarios));
            }
        }

        public ObservableCollection<Categorias> Categoria
        {
            get => _categorias;
            set
            {
                _categorias = value;
                OnPropertyChanged(nameof(Categoria));
            }
        }

        public ICommand SaveComamnd { get; }

        public ConfiguracoesViewModel()
        {
            SaveComamnd = new RelayCommand(async () => await SaveDataAsync());
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            Funcionarios = new ObservableCollection<Funcionario>(await Funcionario.GetFuncionarios(funcionariosFile));
            Categoria = new ObservableCollection<Categorias>(await Categorias.GetCategoriasAsync(categoriasFile));
        }

        private async Task SaveDataAsync()
        {
            await Funcionario.SetFuncionarios(funcionariosFile, new List<Funcionario>(Funcionarios));
            await Categorias.SetCategoriasAsync(categoriasFile, new List<Categorias>(Categoria));
            MessageBox.Show("Dados salvos com sucesso!", "Salvamento", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
