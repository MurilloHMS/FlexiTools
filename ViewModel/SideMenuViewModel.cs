using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using FlexiTools.Model;
using System.Windows.Markup.Localizer;

namespace FlexiTools.ViewModel
{
    internal class SideMenuViewModel : ViewModelBase
    {
        public ObservableCollection<SideMenu> SideMenus { get; set; }


        private Frame _navigationFrame;

        public Frame NavigationFrame
        {
            get { return _navigationFrame; }
            set { _navigationFrame = value; }
        }

        public ICommand ButtonCommand { get; }
        public SideMenuViewModel()
        {
            ButtonCommand = new RelayCommand(OnButtonCommandExecute);

            SideMenus = new ObservableCollection<SideMenu>
            {
                //new SideMenu
                //{
                //    Name = "Cadastros",
                //    SubMenus = new List<SideMenu>
                //    {
                //        new SideMenu{Name = "Boa Solução"}
                //    }
                //},
                new SideMenu
                {
                    Name = "Dados Cartão",
                    SubMenus = new List<SideMenu>
                    {
                        new SideMenu{Name = "Separar Dados"},
                        new SideMenu{Name = "Coletar Dados"},
                    }
                },
                new SideMenu
                {
                    Name = "Ferramentas",
                    SubMenus = new List<SideMenu>
                    {
                        new SideMenu{Name ="Separar PDF"},
                        new SideMenu{Name ="Calcular Alfa Transportes"},
                        new SideMenu{Name = "Coletar ICMS"}

                    }
                },
                new SideMenu
                {
                    Name = "Abastecimentos",
                    SubMenus = new List<SideMenu>
                    {
                        new SideMenu{Name = "Separar Informações Motoristas"},
                    }
                },
                new SideMenu
                {
                    Name = "Configurações",
                    SubMenus = new List<SideMenu>
                    {
                        new SideMenu{Name = "Cadastros"},
                        new SideMenu{Name = "Disparar Emails"}
                    }
                }
            };
        }

        private void OnButtonCommandExecute(object parameter)
        {
            string subcategoryName = parameter as string;
            if (subcategoryName != null && _navigationFrame != null)
            {
                switch (subcategoryName)
                {
                    case "Separar Dados":
                        _navigationFrame.Navigate(new Uri("Pages/DadosCartoes.xaml", UriKind.Relative));
                        break;
                    case "Coletar Dados":
                        _navigationFrame.Navigate(new Uri("Pages/UploadDadosFuncionarios.xaml", UriKind.Relative));
                        break;
                    case "Separar PDF":
                        _navigationFrame.Navigate(new Uri("Pages/SepararPDF.xaml", UriKind.Relative));
                        break;
                    case "Calcular Alfa Transportes":
                        _navigationFrame.Navigate(new Uri("Pages/CalculosAlfaTransportes.xaml", UriKind.Relative));
                        break;
                    case "Separar Informações Motoristas":
                        _navigationFrame.Navigate(new Uri("Pages/UploadDadosAbastecimentos.xaml", UriKind.Relative));
                        break;
                    case "Cadastros":
                        _navigationFrame.Navigate(new Uri("Pages/Configuracoes.xaml", UriKind.Relative));
                        break;
                    case "Boa Solução":
                        _navigationFrame.Navigate(new Uri("Pages/BoaSolucaoView.xaml", UriKind.Relative));
                        break;
                    case "Disparar Emails":
                        _navigationFrame.Navigate(new Uri("Pages/EnvioEmailsView.xaml", UriKind.Relative));
                        break;
                    case "Coletar ICMS":
                        _navigationFrame.Navigate(new Uri("Pages/DadosIcmsView.xaml", UriKind.Relative));
                        break;
                    default:
                        MessageBox.Show($"Página para {subcategoryName} não encontrada.");
                        break;
                }
            }
        }
    }
}
