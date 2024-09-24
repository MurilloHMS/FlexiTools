using FlexiTools.Model;
using Microsoft.Win32;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace FlexiTools.ViewModel
{
    internal class SepararPdfViewModel : ViewModelBase
    {
        private ObservableCollection<SepararPDF> _pages;
        private string _pdfFileName;
        private string _pdfDirectory;

        public ObservableCollection<SepararPDF> Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                OnPropertyChanged(nameof(Pages));
            }
        }

        public ICommand OpenFiles { get; }
        public ICommand SaveArchives { get; }

        public SepararPdfViewModel()
        {
            OpenFiles = new RelayCommand(async _ => await LoadDataAsync());
            SaveArchives = new RelayCommand(async _ => await SaveFiles(), _ => CanSave());
        }



        private async Task LoadDataAsync()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Arquivos PDF|*.pdf"
            };

            if (ofd.ShowDialog() != true) { return; }
            _pdfFileName = ofd.FileName;
            Pages = new ObservableCollection<SepararPDF>(GetSeparatePdfByPage(_pdfFileName));
        }

        private async Task SaveFiles()
        {
            List<SepararPDF> pages = new List<SepararPDF>();
            foreach (var page in Pages)
            {
                pages.Add(page);
            }
            OpenFolderDialog ofd = new OpenFolderDialog();

            if (ofd.ShowDialog() != true) { return; }
            _pdfDirectory = ofd.FolderName;
            SeparatedPdfByPage(_pdfFileName, _pdfDirectory, pages);
        }

        private bool CanSave()
        {
            return Pages != null;
        }

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
                    archives.Add(new SepararPDF { Nome = $"Pagina {i}" });
                }

                return archives ?? new List<SepararPDF>();
            }
            catch (Exception)
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
