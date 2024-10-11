namespace FlexiTools.Model
{
    public class Produtos
    {
        public int Id { get; set; }        
        public string? CodigoSistema { get; set; }
        public string? Produto { get; set; }
        public decimal Formula { get; set; }
        public decimal Quantidade { get; set; }
        public string? Estoque { get; set; }
        public string? Tributacao { get; set; }
    }
}