using ClosedXML.Excel;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace FlexiTools.Model
{
    internal class Abastecimentos
    {
        public string? DataAbastecimento { get; set; }
        public string? Data { get; set; }
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
        public string? FabricanteVeiculo { get; set; }
        public double? ConsumoUrbanoAlcool { get; set; }
        public double? ConsumoRodoviárioAlcool { get; set; }
        public double? ConsumoUrbanoGasolina { get; set; }
        public double? ConsumoRodoviárioGasolina { get; set; }
        public string? NomeDoMotorista { get; set; }
        public string? SetorDoMotorista { get; set; }
        public string? Gerente {  get; set; }
        public string? Email { get; set; }
        public string? LatitudeElongitudeDispositivo { get; set; }
        public string? latitudeElogitudePosto { get; set; }
        public decimal? HodometroAtual { get; set; }
        public decimal? HodometroAnterior { get; set; }
        public decimal? DiferencaHodometro { get; set; }
        public double? MediaKm { get; set; }
        public string? MeioDePagamento { get; set; }
        public string? Produto { get; set; }
        public decimal? Litros { get; set; }
        public decimal? VlrLitro { get; set; }
        public decimal? VlrTotalProdutos { get; set; }
        public decimal? VlrTotalValidado { get; set; }
        public string? Validacao { get; set; }

        
    }
}
