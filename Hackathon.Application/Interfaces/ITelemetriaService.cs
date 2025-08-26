namespace Hackathon.Application.Interfaces;

/// <summary>
/// Interface para serviço de telemetria
/// </summary>
public interface ITelemetriaService
{
    /// <summary>
    /// Registra métrica de forma fire-and-forget
    /// </summary>
    Task RegistrarMetricaAsync(
        string nomeApi, 
        string endpoint, 
        long tempoResposta, 
        bool sucesso, 
        int statusCode,
        CancellationToken cancellationToken = default);
}
