using FlexiTools.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FlexiTools.ViewModel
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

#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere adicionar o modificador "obrigatório" ou declarar como anulável.
        public ConfiguracoesViewModel()
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere adicionar o modificador "obrigatório" ou declarar como anulável.
        {
            SaveComamnd = new RelayCommand(async _ => await SaveDataAsync());
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
