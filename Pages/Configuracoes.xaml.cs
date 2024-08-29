using FlexiTools.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlexiTools.Pages
{
    public partial class Configuracoes : Page
    {
        private readonly string funcionariosFile = "funcionarios.json";
        private readonly string categoriasFile = "Categorias.json";

        public Configuracoes()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var funcionarios = dgvFuncionarios.ItemsSource as List<Funcionario>;
                Funcionario.SetFuncionarios(funcionariosFile, funcionarios);

                var categorias = dgvCategorias.ItemsSource as List<Categorias>;
                Categorias.SetCategoriasAsync(categoriasFile, categorias);

                MessageBox.Show("Dados salvos com sucesso", "Salvar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {

                throw;
            }
            

        }

        private async void Page_Initialized(object sender, EventArgs e)
        {
            var funcionarios = await Funcionario.GetFuncionarios(funcionariosFile);
            dgvFuncionarios.ItemsSource = funcionarios;

            var categorias = await Categorias.GetCategoriasAsync(categoriasFile);
            dgvCategorias.ItemsSource = categorias;
        }
    }
}
