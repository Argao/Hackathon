using Hackathon.Application.DTOs.Requests;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Interfaces;

public interface ISimulacaoService
{
    Task<SimulacaoResponseDTO?> RealizarSimulacaoAsync(SimulacaoRequestDTO request, CancellationToken ct);
    Task<PagedResponse<ListarSimulacoesDTO>> ListarPaginadoAsync(PagedRequest request, CancellationToken ct);
    Task<VolumeSimuladoResponseDTO> ObterVolumeSimuladoPorDiaAsync(DateOnly dataReferencia, CancellationToken ct);
}
