﻿using Microsoft.Win32;
using ClosedXML.Excel;
using System.Windows;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace flexiTools.Model
{
    public class Cartao
    {
        private static readonly string[] CartaoIdentifiers = new[]
        {
            "ARNALDO CESAR DA SILVA - FINAL 552640******5537",
            "FERNANDO TERUYUKI KINUKAWA - FINAL 552640******1383",
            "JACKSON ALBERTO RUIZ - FINAL 552640******1763",
            "FERNANDA FATTORI RAMALHO - FINAL 552640******3477",
            "TIAGO DE MELLO SIQUEIRA - FINAL 552640******2770",
            "EDOMAR HILARIO DA COSTA - FINAL 552640******5181",
            "JOSE RICARDO VILELA DE OLI - FINAL 552640******0529",
            "ROGGER BACH GOLDINHO - FINAL 552640******7443",
            "AILA MARIA SEREFIM - FINAL 552640******9511",
            "EDILENE SIMONELLI DE CARVA - FINAL 552640******9934",
            "NAZIR ABRAO FILHO - FINAL 552640******5760",
            "WAGNER DA SILVA SANTOS - FINAL 552640 * *****2737",
            "GUSTAVO BACELAR MOREIRA DE - FINAL 552640******2953",
            "RODRIGO LEITE DOMINGUES - FINAL 552640******8827"
        };

        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string NumCartao { get; set; }

        StringBuilder result = new StringBuilder();

        public string GetCartaoList()
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

            string filePath = ofd.FileName;
            var dadosFuncionarios = new Dictionary<string, List<(DateTime Data, string Descricao, decimal Valor)>>();

            try
            {
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    var sheet = workbook.Worksheet(1);

                    string currentFuncionario = null;
                    bool coleta = false;

                    foreach (var row in sheet.RowsUsed())
                    {
                        string rowData = string.Join("\t", row.CellsUsed().Select(cell => cell.GetValue<string>()));

                        if (string.IsNullOrWhiteSpace(rowData)) continue;

                        if (CartaoIdentifiers.Any(identifier => rowData.Contains(identifier)))
                        {
                            currentFuncionario = rowData;
                            if (!dadosFuncionarios.ContainsKey(currentFuncionario))
                            {
                                dadosFuncionarios[currentFuncionario] = new List<(DateTime Data, string Descricao, decimal Valor)>();
                            }
                        }

                        if (rowData.Contains("data"))
                        {
                            coleta = true;
                        }
                        else if (rowData.Contains("Total de lançamentos nacionais") || rowData.Contains("Lançamentos nacionais"))
                        {
                            coleta = false;
                        }

                        if (coleta && currentFuncionario != null)
                        {
                            var columns = rowData.Split('\t');
                            if (columns.Length >= 3)
                            {
                                if (DateTime.TryParse(columns[0], out DateTime data))
                                {
                                    if (decimal.TryParse(columns[2], out decimal valor))
                                    {
                                        dadosFuncionarios[currentFuncionario].Add((data, columns[1], valor));
                                    }
                                }
                            }
                        }
                    }
                }

               

                foreach (var funcionario in dadosFuncionarios)
                {
                    result.AppendLine($"Funcionário: {funcionario.Key}");
                    foreach (var (data, descricao, valor) in funcionario.Value)
                    {
                        result.AppendLine($"{data.ToShortDateString()}\t{descricao}\t{valor:C}");
                    }
                    result.AppendLine();
                }

                Clipboard.SetText(result.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ler o arquivo Excel: {ex.Message}");
                return string.Empty;
            }

            return result.ToString();
        }
    }
}
