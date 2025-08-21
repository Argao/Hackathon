namespace Hackathon.Domain.Entities;

public class Parcela
{
    public Guid IdResultado { get; init; }
    public int Numero { get; set; }
    public decimal ValorPrestacao { get; set; }
    public decimal ValorAmortizacao { get; set; }
    public decimal ValorJuros { get; set; }
    public ResultadoSimulacao Resultado { get; set; } = null!;
}
