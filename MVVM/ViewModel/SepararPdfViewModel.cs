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

namespace FlexiTools.MVVM.ViewModel
{
    internal class SepararPdfViewModel : ViewModelBase
    {
        private ObservableCollection<SepararPDF> _pages;

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

        public SepararPdfViewModel()
        {
            OpenFiles = new RelayCommand(async () => await LoadDataAsync());
        }

        private async Task LoadDataAsync()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() != true)
            {
                return;
            }
            Pages = new ObservableCollection<SepararPDF>(SepararPDF.GetSeparatePdfByPage(ofd.FileName, Path.GetDirectoryName(ofd.FileName)));
        }

        private async Task SaveFiles()
        {
            
        }
    }
}
