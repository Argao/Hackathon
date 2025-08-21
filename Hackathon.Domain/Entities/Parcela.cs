using Hackathon.Domain.ValueObjects;

namespace Hackathon.Domain.Entities;

public class Parcela
{
    public Guid IdResultado { get; init; }
    public int Numero { get; set; }
    public ValorMonetario ValorPrestacao { get; set; }
    public ValorMonetario ValorAmortizacao { get; set; }
    public ValorMonetario ValorJuros { get; set; }
    public ResultadoSimulacao Resultado { get; set; } = null!;
}
