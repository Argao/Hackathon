namespace Hackathon.Domain.Entities;

public class VolumeSimuladoAgregado
{
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public decimal TaxaMediaJuro { get; set; }
    public decimal ValorMedioPrestacao { get; set; }
    public decimal ValorTotalDesejado { get; set; }
    public decimal ValorTotalCredito { get; set; }
}
