namespace Hackathon.Domain.Interfaces.Services;

/// <summary>
/// Interface para serviço de integração com Azure EventHub
/// </summary>
public interface IEventHubService
{
    /// <summary>
    /// Envia dados de simulação para o EventHub de forma assíncrona
    /// </summary>
    /// <param name="simulacaoData">Dados da simulação em formato JSON</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task EnviarSimulacaoAsync(string simulacaoData, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Envia dados de simulação para o EventHub de forma assíncrona usando um objeto
    /// </summary>
    /// <param name="simulacao">Objeto da simulação a ser serializado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task EnviarSimulacaoAsync<T>(T simulacao, CancellationToken cancellationToken = default) where T : class;
}
