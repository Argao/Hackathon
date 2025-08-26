using Hackathon.Application.Interfaces;
using Hackathon.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Publisher genérico que abstrai detalhes de implementação de eventos
/// DIP: Depende de abstração IEventHubService, não de implementação concreta
/// SRP: Apenas publicação de eventos em background
/// </summary>
public class EventPublisher : IEventPublisher
{
    private readonly IEventHubService _eventHubService;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IEventHubService eventHubService, ILogger<EventPublisher> logger)
    {
        _eventHubService = eventHubService;
        _logger = logger;
    }

    public void PublishAsync<T>(T eventData) where T : class
    {
        // Fire-and-forget usando ThreadPool para não bloquear
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                _eventHubService.EnviarSimulacao(eventData);
                _logger.LogInformation("✅ Evento publicado com sucesso: {EventType}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Falha ao publicar evento: {EventType}", typeof(T).Name);
            }
        }, null);
    }
}