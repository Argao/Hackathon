using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Interface para serviço de telemetria
/// </summary>
public interface ITelemetriaService
{
    
    Task RegistrarMetricaAsync(
        string nomeApi, 
        string endpoint, 
        long tempoResposta, 
        bool sucesso, 
        int statusCode,
        CancellationToken cancellationToken = default);
    
    Task<TelemetriaFinalResponseDTO> ObterTelemetriaPorDataAsync(
        DateOnly dataReferencia, 
        CancellationToken cancellationToken = default);
}
