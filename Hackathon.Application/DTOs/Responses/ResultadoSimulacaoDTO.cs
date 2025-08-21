using Hackathon.Domain.Enums;

namespace Hackathon.Application.DTOs.Responses;

public class ResultadoSimulacaoDTO
{
    public SistemaAmortizacao Tipo { get; set; }
    public ICollection<ParcelaDTO> Parcelas { get; set; } = [];
}
