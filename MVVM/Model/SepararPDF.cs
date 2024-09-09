using DocumentFormat.OpenXml.Packaging;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;


namespace FlexiTools.MVVM.Model
{
    internal class SepararPDF
    {
        public string Nome {  get; set; }

        public static List<SepararPDF> GetSeparatePdfByPage(string inputPdfPath)
        {        
            try
            {
                if (!File.Exists(inputPdfPath))
                {
                    MessageBox.Show("Arquivo PDF não encontrado.", "Error: File has not found", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<SepararPDF>();
                }

                PdfDocument inputDocument = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import);
                List<SepararPDF> archives = new List<SepararPDF>();
                for (int i = 0; i < inputDocument.Pages.Count; i++)
                {
                    archives.Add(new SepararPDF { Nome = $"Pagina {i}"});
                }

                return archives ?? new List<SepararPDF>();
            }
            catch(Exception)
            {
                return new List<SepararPDF>();
            }
        }

        public static void SeparatedPdfByPage(string inputPdfPath, string outputFolder, List<SepararPDF> lista)
        {
            if (!File.Exists(inputPdfPath))
            {
                MessageBox.Show("Arquivo PDF não encontrado.", "Error: File has not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                PdfDocument inputDocument = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import);

                int pageNum = 0;

                for (int pageNumber = 0; pageNumber < inputDocument.PageCount; pageNumber++)
                {
                    PdfDocument outputDocument = new PdfDocument();
                    outputDocument.AddPage(inputDocument.Pages[pageNumber]);
                    string outputFilePath = Path.Combine(outputFolder, $"{lista[pageNumber].Nome}.pdf");
                    outputDocument.Save(outputFilePath);
                    outputDocument.Close();
                    pageNum++;
                }

                MessageBox.Show($"Arquivos Separados em {pageNum} arquivos e salvos em : \n\n {outputFolder}", "Salvamento de Arquivos", MessageBoxButton.OK, MessageBoxImage.Information);

                inputDocument.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}
