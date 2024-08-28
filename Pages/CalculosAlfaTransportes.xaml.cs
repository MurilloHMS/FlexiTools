using System.Drawing.Printing;
using System.Drawing;
using System.Windows.Controls;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Windows;
using System.Text;

namespace FlexiTools.Pages
{
    public partial class CalculosAlfaTransportes : Page
    {

        private PrintDocument printDocument;
        private string? textToprint;

        public CalculosAlfaTransportes()
        {
            InitializeComponent();
            printDocument = new PrintDocument();
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textToprint))
            {
                MessageBox.Show("Não há texto para imprimir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PrintDialog printDialog = new PrintDialog();

            TextBlock textBlock = new TextBlock
            {
                Text = textToprint,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 10,
                Margin = new Thickness(10)
            };

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(textBlock, "Impressão de Texto");
            }
        }

        private void btnAbrir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Arquivos XML (*.xml)|*.xml|Todos os Arquivos (*.*)|*.*",
                    Multiselect = true,
                    RestoreDirectory = true,
                };

                if(ofd.ShowDialog() != true)
                {
                    MessageBox.Show("Nenhum Arquivo Selecionado");
                    return;
                }

                StringBuilder resultado = new StringBuilder();
                decimal totalValorPrest = 0m;

                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    string filePath = ofd.FileNames[i];
                    var (resultadoArquivo, valorPrest) = ProcessarArquivoXML(filePath);
                    resultado.Append(resultadoArquivo);
                    if (i < ofd.FileNames.Length - 1)
                    {
                        resultado.Append(" + ");
                    }
                    resultado.AppendLine();
                    totalValorPrest += valorPrest;
                }

                resultado.AppendLine();
                resultado.AppendLine($"Total: {totalValorPrest.ToString("C", CultureInfo.CurrentCulture)}");
                txtResultadoDosCalculos.Text = resultado.ToString();

                textToprint = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private (string, decimal) ProcessarArquivoXML(string filePath)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(filePath);
                XNamespace ns = "http://www.portalfiscal.inf.br/cte";

                var vPrest = xmlDoc.Descendants(ns + "vPrest").FirstOrDefault();
                decimal vTPrest = 0m;

                if (vPrest != null)
                {
                    string? vTPrestString = vPrest.Element(ns + "vTPrest")?.Value;

                    vTPrest = !string.IsNullOrEmpty(vTPrestString) && decimal.TryParse(vTPrestString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal vTPrestValue) ? vTPrestValue : 0m;
                    return ($"{vTPrest:C} ", vTPrest);
                }
                else
                {
                    return ($"", 0m);
                }
            }
            catch (Exception)
            {
                return ($" ", 0m);
            }
        }
    }
}
