using Hackathon.Application.Commands;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Interface para serviços de simulação
/// </summary>
public interface ISimulacaoService
{
    /// <summary>
    /// Realiza uma simulação de crédito
    /// </summary>
    Task<Result<SimulacaoResult>> RealizarSimulacaoAsync(RealizarSimulacaoCommand command, CancellationToken ct);

    /// <summary>
    /// Lista simulações de forma paginada
    /// </summary>
    Task<Result<PagedResult<SimulacaoResumoResult>>> ListarSimulacoesAsync(ListarSimulacoesQuery query, CancellationToken ct);

    /// <summary>
    /// Obtém volume simulado por data
    /// </summary>
    Task<Result<VolumeSimuladoResult>> ObterVolumeSimuladoAsync(ObterVolumeSimuladoQuery query, CancellationToken ct);
}