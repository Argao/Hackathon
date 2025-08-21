using Hackathon.Application.DTOs.Responses;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Interface para serviço de telemetria
/// </summary>
public interface ITelemetriaService
{
    /// <summary>
    /// Registra uma métrica de requisição de forma assíncrona (fire-and-forget)
    /// </summary>
    /// <param name="nomeApi">Nome da API (controller)</param>
    /// <param name="endpoint">Endpoint específico</param>
    /// <param name="tempoResposta">Tempo de resposta em millisegundos</param>
    /// <param name="sucesso">Se a requisição foi bem-sucedida</param>
    /// <param name="statusCode">Código de status HTTP</param>
    /// <param name="ipCliente">IP do cliente (opcional)</param>
    /// <param name="userAgent">User-Agent do cliente (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task RegistrarMetricaAsync(
        string nomeApi, 
        string endpoint, 
        long tempoResposta, 
        bool sucesso, 
        int statusCode,
        string? ipCliente = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém dados de telemetria agregados por data
    /// </summary>
    /// <param name="dataReferencia">Data para consulta</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados de telemetria agregados</returns>
    Task<Result<TelemetriaFinalResponseDTO>> ObterTelemetriaPorDataAsync(
        DateOnly dataReferencia, 
        CancellationToken cancellationToken = default);
}
