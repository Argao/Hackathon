using Hackathon.Application.Queries;
using Hackathon.Application.Results;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Orquestrador responsável apenas por coordenar o processo de telemetria
/// SRP: Uma única responsabilidade - orquestrar consultas de telemetria
/// </summary>
public interface ITelemetriaOrchestrator
{
    Task<TelemetriaResult> ObterTelemetriaAsync(ObterTelemetriaQuery query, CancellationToken cancellationToken);
}
