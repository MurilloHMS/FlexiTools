using FlexiTools.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using ClosedXML.Excel;

namespace FlexiTools.ViewModel
{
    public class DadosIcmsViewModel : ViewModelBase
    {
        private ObservableCollection<ICMS> _dadosIcms;

        public ObservableCollection<ICMS> DadosIcms
        {
            get => _dadosIcms;
            set
            {
                _dadosIcms = value;
                OnPropertyChanged(nameof(DadosIcms));
            }
        }

        public ICommand ImportarDados { get; set; }
        public DadosIcmsViewModel()
        {
            DadosIcms = new ObservableCollection<ICMS>();
            ImportarDados = new RelayCommand(async _ => await ColetarDadosXML());
        }

        private async Task ColetarDadosXML()
        {
            try
            {
                DadosIcms.Clear();
                OpenFileDialog dlg = new OpenFileDialog()
                {
                    Filter = "Arquivos XML (*.xml)|*.xml|Todos os arquivos (*.*)|*.*",
                    Multiselect = true,
                    RestoreDirectory = true,
                };

                if (dlg.ShowDialog() != true)
                {
                    MessageBox.Show("Nenhum Arquivo selecionado");
                    return;
                }

                for(int i = 0; i < dlg.FileNames.Length; i++)
                {
                    string filepath = dlg.FileNames[i];
                    DadosIcms.Add(ProcessarXML(filepath));                    
                }

                SalvarDados();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ICMS ProcessarXML(string path)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(path);
                XNamespace ns = "http://www.portalfiscal.inf.br/nfe";

                var dados = new ICMS();

                var numNfTag = xmlDoc.Descendants(ns + "ide").FirstOrDefault();
                if (numNfTag != null) 
                {
                    dados.nNF = numNfTag.Element(ns + "nNF").Value;
                }

                var icmsTag = xmlDoc.Descendants(ns + "ICMSTot").FirstOrDefault();
                if (icmsTag != null)
                {
                    string icms = icmsTag.Element(ns + "vICMS").Value;
                    dados.vICMS = !string.IsNullOrEmpty(icms) && decimal.TryParse(icms, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) ? valor : 0m;
                }

                return dados;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SalvarDados()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Arquivos Excel (*.xlsx)|*.xlsx",
                Title = "Escolha onde salvar o arquivo"
            };

            if (dialog.ShowDialog() == true)
            {
                string caminhoArquivo = dialog.FileName;

                try
                {
                    await Task.Run(() =>
                    {
                        using(var workbook = File.Exists(caminhoArquivo) ? new XLWorkbook(caminhoArquivo) : new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Count > 0 ? workbook.Worksheet(1) : workbook.Worksheets.Add("Resultado");
                            var ultimaLinha = worksheet.CellsUsed().Any() ? worksheet.CellsUsed().Last().Address.RowNumber : 0;

                            int novaLinha = ultimaLinha + 1;

                            foreach (var row in DadosIcms)
                            {
                                worksheet.Cell(novaLinha, 1).Value = row.nNF;
                                worksheet.Cell(novaLinha, 2).Value = row.vICMS;
                                novaLinha++;
                            }

                            workbook.SaveAs(caminhoArquivo);
                        }
                    });
                    MessageBox.Show("Dados salvos com sucesso.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar dados: {ex.Message}");
                }
            }
        }
    }
}
