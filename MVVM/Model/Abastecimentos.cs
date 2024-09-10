using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlexiTools.MVVM.Model
{
    internal class Abastecimentos
    {
        public DateTime? DataAbastecimento { get; set; }
        public string? StatusEmissaoNFe { get; set; }
        public string? CNPJ { get; set; }
        public string? RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        public string? CNPJEstabelecimento { get; set; }
        public string? RazaoSocialEstabelecimento { get; set; }
        public string? NomeFantasiaEstabelecimetno { get; set; }
        public string? EnderecoEstabelecimento { get; set; }
        public string? CidadeEstabelecimento { get; set; }
        public string? UfEstabelecimento { get; set; }
        public string? PlacaDoVeiculo { get; set; }
        public string? ModeloDoVeiculo { get; set; }
        public string? NomeDoMotorista { get; set; }
        public string? LatitudeElongitudeDispositivo { get; set; }
        public string? latitudeElogitudePosto { get; set; }
        public decimal? HodometroAtual { get; set; }
        public decimal? HodometroAnterior { get; set; }
        public decimal? DiferencaHodometro { get; set; }
        public decimal? MediaKm { get; set; }
        public string? MeioDePagamento { get; set; }
        public string? Produto { get; set; }
        public decimal? Litros { get; set; }
        public decimal? VlrLitro { get; set; }
        public decimal? VlrTotalProdutos { get; set; }
        public decimal? VlrTotalValidado { get; set; }
        public bool? Validacao { get; set; }

        public static async Task<IEnumerable<Abastecimentos>> GetAbastecimentosAsync()
        {
            List<Abastecimentos> abastecimentos = new List<Abastecimentos>();

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Multiselect = false,
                Title = "Selecione a planilha de abastecimento"
            };

            if (ofd.ShowDialog() != true)
            {
                MessageBox.Show("Nenhum arquivo selecionado", "Seleção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return Enumerable.Empty<Abastecimentos>();
            }

            await Task.Run(() =>
            {
                using (XLWorkbook wb = new XLWorkbook(ofd.FileName))
                {
                    var planilha = wb.Worksheet(1);
                    var filedata = planilha.RowsUsed().Skip(1).Select(row => new Abastecimentos
                    {
                        DataAbastecimento = row.Cell(6).TryGetValue<string>(out var dataAbastecimento) ? DateTime.Parse(dataAbastecimento) : null,
                        StatusEmissaoNFe = row.Cell(7).TryGetValue<string>(out var statusEmissaoNFe) ? statusEmissaoNFe : null,
                        CNPJ = row.Cell(8).TryGetValue<string>(out var CNPJ) ? CNPJ : null,
                        RazaoSocial = row.Cell(9).TryGetValue<string>(out var RazaoSocial) ? RazaoSocial : null,
                        NomeFantasia = row.Cell(10).TryGetValue<string>(out var nomeFantasia) ? nomeFantasia : null,
                        CNPJEstabelecimento = row.Cell(11).TryGetValue<string>(out var cnpjEstabelecimento) ? cnpjEstabelecimento : null,
                        RazaoSocialEstabelecimento = row.Cell(12).TryGetValue<string>(out var razaoSocialEstabelecimento) ? razaoSocialEstabelecimento : null,
                        NomeFantasiaEstabelecimetno = row.Cell(13).TryGetValue<string>(out var nomeFantasiaEstabelecimento) ? nomeFantasiaEstabelecimento : null,
                        EnderecoEstabelecimento = row.Cell(14).TryGetValue<string>(out var enderecoEstabelecimento) ? enderecoEstabelecimento : null,
                        CidadeEstabelecimento = row.Cell(15).TryGetValue<string>(out var cidadeEstabelecimento) ? cidadeEstabelecimento : null,
                        UfEstabelecimento = row.Cell(16).TryGetValue<string>(out var ufEstabelecimento) ? ufEstabelecimento : null,
                        PlacaDoVeiculo = row.Cell(17).TryGetValue<string>(out var placaDoVeiculo) ? placaDoVeiculo : null,
                        LatitudeElongitudeDispositivo = row.Cell(23).TryGetValue<string>(out var longitudeLatitudeDispositivo) ? longitudeLatitudeDispositivo : null,
                        latitudeElogitudePosto = row.Cell(24).TryGetValue<string>(out var longitudeLatitudePosto) ? longitudeLatitudePosto : null,
                        HodometroAtual = row.Cell(26).TryGetValue<decimal>(out var hodometroAtual) ? hodometroAtual : null,
                        HodometroAnterior = row.Cell(27).TryGetValue<decimal>(out var hodometroAnterior) ? hodometroAnterior : null,
                        DiferencaHodometro = row.Cell(28).TryGetValue<decimal>(out var diferencaHodometro) ? diferencaHodometro : null,
                        MediaKm = row.Cell(29).TryGetValue<decimal>(out var mediaKM) ? mediaKM : null,
                        MeioDePagamento = row.Cell(33).TryGetValue<string>(out var meioDePagamento) ? meioDePagamento : null,
                        Produto = row.Cell(40).TryGetValue<string>(out var produto) ? produto : null,
                        Litros = row.Cell(41).TryGetValue<decimal>(out var litros) ? litros : null,
                        VlrLitro = row.Cell(42).TryGetValue<decimal>(out var vlrLitro) ? vlrLitro : null,
                        VlrTotalProdutos = row.Cell(43).TryGetValue<decimal>(out var vlrTotalProdutos) ? vlrTotalProdutos : null,
                        VlrTotalValidado = vlrLitro * litros,
                        Validacao = vlrTotalProdutos == vlrLitro * litros ? true : false

                    }).ToList();

                    lock (abastecimentos)
                    {
                        abastecimentos.AddRange(filedata);
                    }
                }
            });

            return abastecimentos;
        }

        public static async Task SaveDataAsync(string path, IEnumerable<Abastecimentos> data)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var workbook = File.Exists(path) ? new XLWorkbook(path) : new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Count > 1 ? workbook.Worksheet(2) : workbook.Worksheets.Add("Dados");

                        var lastLine = worksheet.CellsUsed().Any() ? worksheet.CellsUsed().Last().Address.RowNumber : 0;

                        int newLine = lastLine + 1;

                        foreach (var row in data)
                        {
                            worksheet.Cell(newLine, 1).Value = row.DataAbastecimento;
                            worksheet.Cell(newLine, 2).Value = row.StatusEmissaoNFe;
                            worksheet.Cell(newLine, 3).Value = row.CNPJ;
                            worksheet.Cell(newLine, 4).Value = row.RazaoSocial;
                            worksheet.Cell(newLine, 5).Value = row.NomeFantasia;
                            worksheet.Cell(newLine, 6).Value = row.CNPJEstabelecimento;
                            worksheet.Cell(newLine, 7).Value = row.RazaoSocialEstabelecimento;
                            worksheet.Cell(newLine, 8).Value = row.NomeFantasiaEstabelecimetno;
                            worksheet.Cell(newLine, 9).Value = row.EnderecoEstabelecimento;
                            worksheet.Cell(newLine, 10).Value = row.CidadeEstabelecimento;
                            worksheet.Cell(newLine, 11).Value = row.UfEstabelecimento;
                            worksheet.Cell(newLine, 12).Value = row.PlacaDoVeiculo;
                            worksheet.Cell(newLine, 13).Value = row.ModeloDoVeiculo;
                            worksheet.Cell(newLine, 14).Value = row.NomeDoMotorista;
                            worksheet.Cell(newLine, 15).Value = row.LatitudeElongitudeDispositivo;
                            worksheet.Cell(newLine, 16).Value = row.latitudeElogitudePosto;
                            worksheet.Cell(newLine, 17).Value = row.HodometroAtual;
                            worksheet.Cell(newLine, 18).Value = row.HodometroAnterior;
                            worksheet.Cell(newLine, 19).Value = row.DiferencaHodometro;
                            worksheet.Cell(newLine, 20).Value = row.MediaKm;
                            worksheet.Cell(newLine, 21).Value = row.MeioDePagamento;
                            worksheet.Cell(newLine, 22).Value = row.Produto;
                            worksheet.Cell(newLine, 23).Value = row.Litros;
                            worksheet.Cell(newLine, 24).Value = row.VlrLitro;
                            worksheet.Cell(newLine, 25).Value = row.VlrTotalProdutos;
                            worksheet.Cell(newLine, 26).Value = row.VlrTotalValidado;
                            worksheet.Cell(newLine, 27).Value = $"{row.Validacao}";
                            newLine++;
                        }

                        workbook.SaveAs(path);
                        MessageBox.Show("Dados Salvos com Sucesso!", "Dados Salvos", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                });
            }
            catch (IOException ex)
            {

                MessageBox.Show($"Ocorreu um erro ao salvar o arquivo: {ex.Message}", "IOException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro inesperado: {ex.Message}", "Error Handle", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }        
    }
}
