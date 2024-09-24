﻿using ClosedXML.Excel;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace FlexiTools.Model
{
    public class Cartao
    {
        public string? Nome { get; set; }
        public DateTime? Data { get; set; }
        public string? Descricao { get; set; }
        public decimal? Valor { get; set; }
        public string? Categoria { get; set; }
        public string? Cliente { get; set; }

    }
}
