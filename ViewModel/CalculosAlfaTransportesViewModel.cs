using Microsoft.Win32;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace FlexiTools.ViewModel
{
    public class CalculosAlfaTransportesViewModel : ViewModelBase
    {
        private string _resultadoDosCalculos;
        private string _textToPrint;

        public string ResultadoDosCalculos
        {
            get => _resultadoDosCalculos;
            set
            {
                _resultadoDosCalculos = value;
                OnPropertyChanged(nameof(ResultadoDosCalculos));
            }
        }

        public ICommand AbrirCommand { get; }
        public ICommand ImprimirCommand { get; }

        public CalculosAlfaTransportesViewModel()
        {
            AbrirCommand = new RelayCommand(ExecuteAbrirCommand);
            ImprimirCommand = new RelayCommand(ExecuteImprimirCommand, CanPrint);
        }

        private void ExecuteAbrirCommand()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Arquivos XML (*.xml)|*.xml|Todos os Arquivos (*.*)|*.*",
                    Multiselect = true,
                    RestoreDirectory = true,
                };

                if (ofd.ShowDialog() != true)
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
                ResultadoDosCalculos = resultado.ToString();

                _textToPrint = resultado.ToString();
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
        private void ExecuteImprimirCommand()
        {
            if (string.IsNullOrEmpty(_textToPrint))
            {
                MessageBox.Show("Não há texto para imprimir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PrintDialog printDialog = new PrintDialog();

            TextBlock textBlock = new TextBlock
            {
                Text = _textToPrint,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 10,
                Margin = new Thickness(10)
            };

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(textBlock, "Impressão de Texto");
            }
        }

        private bool CanPrint()
        {
            return !string.IsNullOrEmpty(_textToPrint);
        }
    }
}