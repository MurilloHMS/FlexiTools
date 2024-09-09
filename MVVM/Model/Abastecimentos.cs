using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlexiTools.MVVM.Model
{
    internal class Abastecimentos
    {
        public string? DataAbastecimento { get; set; }
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
        public decimal? DiferencaHodometro {  get; set; }
        public decimal? MediaKm {  get; set; }
        public string? MeioDePagamento { get; set; }
        public string? Produto { get; set; }
        public decimal? Litros {  get; set; }
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
                        DataAbastecimento = row.Cell(6).TryGetValue<string>(out var dataAbastecimento) ? dataAbastecimento : null,
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
                        VlrTotalValidado =  vlrLitro * litros,
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
    }
}
