using FlexiTools.MVVM.Model;
using Microsoft.Extensions.FileProviders.Physical;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private async void btnDados_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            
            var progress = new Progress<string>(message =>
            {
                sb.AppendLine(message);
                txtConteudo.Text = sb.ToString();
            });

            try
            {
                await Cartao.GerarPlanilhasPorFuncionarioAsync(progress);
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Erro: {ex.Message}");
                txtConteudo.Text = sb.ToString();
            }
        }


    }
}
