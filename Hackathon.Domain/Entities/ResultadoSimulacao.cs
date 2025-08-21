using Hackathon.Domain.Enums;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Domain.Entities;

public class ResultadoSimulacao
{
    public Guid IdResultado { get; init; } = Guid.NewGuid();
    public Guid IdSimulacao { get; set; }
    public SistemaAmortizacao Tipo { get; set; }
    public Simulacao Simulacao { get; set; } = null!;
    public ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
    
    public ValorMonetario ValorTotal { get; set; }
}
