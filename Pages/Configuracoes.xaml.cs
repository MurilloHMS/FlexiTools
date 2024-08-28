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
    /// <summary>
    /// Interação lógica para Configuracoes.xam
    /// </summary>
    public partial class Configuracoes : Page
    {
        public Configuracoes()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            string file = "funcionarios.json";
            var dados = dgvFuncionarios.ItemsSource as List<Funcionario>;
            Funcionario func = new Funcionario();
            func.SetFuncionarios(file, dados);
        }

        private async void Page_Initialized(object sender, EventArgs e)
        {
            string file = "funcionarios.json";
            Funcionario func = new Funcionario();
            var funcionarios = await func.GetFuncionarios(file);
            dgvFuncionarios.ItemsSource = funcionarios;            
        }
    }
}
