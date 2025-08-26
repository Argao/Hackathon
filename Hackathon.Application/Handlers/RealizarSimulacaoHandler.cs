using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Results;
using MediatR;

namespace Hackathon.Application.Handlers;

/// <summary>
/// Handler minimalista - responsabilidade única: ser um ponto de entrada para o MediatR
/// SRP: Apenas delega para o orquestrador
/// </summary>
public class RealizarSimulacaoHandler : IRequestHandler<RealizarSimulacaoCommand, SimulacaoResult>
{
    private readonly ISimulacaoOrchestrator _orchestrator;

    public RealizarSimulacaoHandler(ISimulacaoOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<SimulacaoResult> Handle(RealizarSimulacaoCommand request, CancellationToken cancellationToken)
    {
        return await _orchestrator.RealizarSimulacaoAsync(request, cancellationToken);
    }
}