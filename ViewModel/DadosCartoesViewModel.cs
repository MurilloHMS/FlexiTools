using FlexiTools.Model;
using System.Text;
using System.Windows.Input;

namespace FlexiTools.ViewModel
{
    internal class DadosCartoesViewModel : ViewModelBase
    {

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
                await Cartao.GerarPlanilhasPorFuncionarioAsync(progress);
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Erro: {ex.Message}");
                Conteudo = sb.ToString();
            }
        }
    }
}
