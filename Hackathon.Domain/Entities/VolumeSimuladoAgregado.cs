using Hackathon.Domain.ValueObjects;

namespace Hackathon.Domain.Entities;

public class VolumeSimuladoAgregado
{
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public TaxaJuros TaxaMediaJuro { get; set; }
    public ValorMonetario ValorMedioPrestacao { get; set; }
    public ValorMonetario ValorTotalDesejado { get; set; }
    public ValorMonetario ValorTotalCredito { get; set; }
}
