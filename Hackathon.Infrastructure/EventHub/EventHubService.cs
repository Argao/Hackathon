using System.Text;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Hackathon.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.EventHub;

/// <summary>
/// Implementação do serviço de integração com Azure EventHub
/// </summary>
public class EventHubService : IEventHubService, IDisposable
{
    private readonly EventHubProducerClient _producer;
    private readonly ILogger<EventHubService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed = false;

    public EventHubService(IConfiguration configuration, ILogger<EventHubService> logger)
    {
        _logger = logger;
        
        // Configurações de serialização JSON para otimizar performance
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false, // Reduz tamanho da mensagem
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var connectionString = configuration.GetConnectionString("EventHub")
            ?? throw new InvalidOperationException("EventHub connection string não configurada");

        // Configurações do producer para performance
        var producerOptions = new EventHubProducerClientOptions
        {
            RetryOptions = new EventHubsRetryOptions
            {
                MaximumRetries = 3,
                MaximumDelay = TimeSpan.FromSeconds(10),
                Mode = EventHubsRetryMode.Exponential
            }
        };

        _producer = new EventHubProducerClient(connectionString, producerOptions);
    }

    /// <summary>
    /// Envia dados de simulação para o EventHub de forma assíncrona
    /// </summary>
    public async Task EnviarSimulacaoAsync(string simulacaoData, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(EventHubService));
        
        try
        {
            // Criar evento com dados comprimidos para melhorar performance
            var eventData = new EventData(Encoding.UTF8.GetBytes(simulacaoData));
            
            // Adicionar propriedades para facilitar filtros no consumidor
            eventData.Properties["EventType"] = "SimulacaoRealizada";
            eventData.Properties["Timestamp"] = DateTimeOffset.UtcNow.ToString("O");
            eventData.Properties["Source"] = "HackathonAPI";

            // Enviar evento de forma assíncrona
            await _producer.SendAsync(new[] { eventData }, cancellationToken);
            
            _logger.LogInformation("Simulação enviada com sucesso para o EventHub");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Envio para EventHub foi cancelado: {ErrorMessage}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar simulação para o EventHub: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Envia dados de simulação para o EventHub de forma assíncrona usando um objeto
    /// </summary>
    public async Task EnviarSimulacaoAsync<T>(T simulacao, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(simulacao);
        
        var json = JsonSerializer.Serialize(simulacao, _jsonOptions);
        await EnviarSimulacaoAsync(json, cancellationToken);
    }

    /// <summary>
    /// Libera recursos do producer
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _producer?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(5));
            _disposed = true;
        }
    }
}
