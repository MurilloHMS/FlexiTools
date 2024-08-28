﻿using flexiTools.Model;
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

namespace SideBar_Nav.Pages
{
    public partial class UploadDadosFuncionarios : Page
    {
        public UploadDadosFuncionarios()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dados = await Cartao.ObterDadosDasPlanilhasAsync();
            dgvCartoes.ItemsSource = dados;
        }
    }
}
