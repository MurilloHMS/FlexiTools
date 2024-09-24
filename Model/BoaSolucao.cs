namespace FlexiTools.Model
{
    public class BoaSolucao
    {
        public int Id { get; set; }
        public string? Produto { get; set; }
        public decimal CalculoEmbalagem { get; set; }
        public decimal ValorMaoDeObra { get; set; }
        public double Caixas { get; set; }
        public List<Produtos>? produtos { get; set; }
    }
}