using DocumentFormat.OpenXml.Office2010.Drawing;
using FlexiTools.MVVM.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FlexiTools.MVVM.ViewModel
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

        public ICommand OpenFiles {  get;}
        public ICommand SaveArchives { get;}

        public SepararPdfViewModel()
        {
            OpenFiles = new RelayCommand(async () => await LoadDataAsync());
            SaveArchives = new RelayCommand(async () => await SaveFiles());
        }

        

        private async Task LoadDataAsync()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog() != true) { return; }
            _pdfFileName = ofd.FileName;
            Pages = new ObservableCollection<SepararPDF>(SepararPDF.GetSeparatePdfByPage(_pdfFileName));
        }

        private async Task SaveFiles()
        {
            List<SepararPDF> pages = new List<SepararPDF>();
            foreach(var page in Pages)
            {
                pages.Add(page);
            }
            OpenFolderDialog ofd = new OpenFolderDialog();

            if (ofd.ShowDialog() != true) { return; }
            _pdfDirectory = ofd.FolderName;
            SepararPDF.SeparatedPdfByPage(_pdfFileName, _pdfDirectory, pages);
        }
    }
}
