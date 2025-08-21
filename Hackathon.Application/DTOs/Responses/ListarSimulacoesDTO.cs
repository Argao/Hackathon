using Hackathon.Domain.Enums;

namespace Hackathon.Application.DTOs.Responses;

public class ListarSimulacoesDTO
{
    public Guid Id { get; set; }
    public decimal ValorDesejado { get; set; }
    public short PrazoMeses { get; set; }
    public ICollection<ValorTotalParcelasDTO> ResultadoSimulacao { get; set; } = [];
}

public class ValorTotalParcelasDTO
{
    public SistemaAmortizacao Tipo { get; set; }
    public decimal ValorTotal { get; set; }
}
