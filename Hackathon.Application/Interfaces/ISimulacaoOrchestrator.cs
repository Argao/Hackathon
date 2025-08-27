using Hackathon.Application.Commands;
using Hackathon.Application.Results;

namespace Hackathon.Application.Interfaces;

public interface ISimulacaoOrchestrator
{
    Task<SimulacaoResult> RealizarSimulacaoAsync(RealizarSimulacaoCommand command, CancellationToken cancellationToken);
}