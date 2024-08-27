using Microsoft.Win32;
using ClosedXML.Excel;
using System.Windows;
using System.Text;

namespace flexiTools.Model
{
    public class Cartao
    {

        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string NumCartao { get; set; }

        public static string GetCartaoList()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Todos os Arquivos (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() != true)
            {
                MessageBox.Show("Nenhum arquivo selecionado.");
                return string.Empty;
            }

            // Obtém o caminho do arquivo selecionado
            string filePath = ofd.FileName;

            // Cria o StringBuilder para acumular os dados
            StringBuilder nome = new StringBuilder();

            try
            {
                // Carrega o arquivo Excel usando ClosedXML
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    // Obtém a primeira planilha do arquivo
                    var sheet = workbook.Worksheet(1);

                    // Itera sobre as linhas usadas da planilha
                    foreach (var row in sheet.RowsUsed())
                    {
                        // Concatena os valores das células da linha em uma string
                        string rowData = string.Join("\t", row.CellsUsed().Select(cell => cell.GetValue<string>()));
                        nome.AppendLine(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ler o arquivo Excel: {ex.Message}");
                return string.Empty;
            }

            return nome.ToString();
        }
    }
}
