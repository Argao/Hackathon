using Hackathon.Application.Commands;
using Hackathon.Application.Results;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Orquestrador responsável apenas por coordenar o processo de simulação
/// SRP: Uma única responsabilidade - orquestrar
/// </summary>
public interface ISimulacaoOrchestrator
{
    Task<SimulacaoResult> RealizarSimulacaoAsync(RealizarSimulacaoCommand command, CancellationToken cancellationToken);
}