namespace Hackathon.Domain.Interfaces.Services;

/// <summary>
/// Interface para serviço de integração com Azure EventHub
/// </summary>
public interface IEventHubService
{

    Task EnviarSimulacaoAsync(string simulacaoData, CancellationToken cancellationToken = default);
    

    Task EnviarSimulacaoAsync<T>(T simulacao, CancellationToken cancellationToken = default) where T : class;
    
    void EnviarSimulacao(string simulacaoData);
    void EnviarSimulacao<T>(T simulacao) where T : class;
}
