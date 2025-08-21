using Hackathon.Domain.Enums;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Domain.Entities;

public sealed class Simulacao
{
    public Guid IdSimulacao { get; init; } = Guid.NewGuid();
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public TaxaJuros TaxaJuros { get; set; }
    public ValorMonetario ValorDesejado { get; set; }
    public short PrazoMeses { get; set; }
    public DateOnly DataReferencia { get; set; }
    public string EnvelopJson { get; set; } = string.Empty;
    
    public ICollection<ResultadoSimulacao> Resultados { get; set; } = new List<ResultadoSimulacao>();
}
