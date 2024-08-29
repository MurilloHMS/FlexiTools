using FlexiTools.MVVM.Model;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;


namespace FlexiTools.Pages
{
    public partial class UploadDadosFuncionarios : Page
    {
        public UploadDadosFuncionarios()
        {
            InitializeComponent();
        }

        private async void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            var dados = await Cartao.ObterDadosDasPlanilhasAsync();
            dgvCartoes.ItemsSource = dados;
        }

        private async void btnSalvar(object sender, RoutedEventArgs e)
        {
            var dados = dgvCartoes.ItemsSource as IEnumerable<Cartao>;

            if (dados == null || !dados.Any())
            {
                MessageBox.Show("Não há dados para salvar.");
                return;
            }

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
                    await Cartao.SalvarDados(caminhoArquivo, dados);
                    MessageBox.Show("Dados salvos com sucesso.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar dados: {ex.Message}");
                }
            }
        }
    }
}
