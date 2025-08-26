using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using MediatR;

namespace Hackathon.Application.Handlers;

/// <summary>
/// Handler minimalista - responsabilidade única: ser um ponto de entrada para o MediatR
/// SRP: Apenas delega para o orquestrador
/// </summary>
public class ObterTelemetriaHandler : IRequestHandler<ObterTelemetriaQuery, TelemetriaResult>
{
    private readonly ITelemetriaOrchestrator _orchestrator;

    public ObterTelemetriaHandler(ITelemetriaOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<TelemetriaResult> Handle(ObterTelemetriaQuery request, CancellationToken cancellationToken)
    {
        return await _orchestrator.ObterTelemetriaAsync(request, cancellationToken);
    }
}
