namespace Hackathon.Application.Interfaces;

/// <summary>
/// Publisher genérico para eventos
/// SRP: Publicação de eventos
/// OCP: Extensível para novos tipos de evento
/// </summary>
public interface IEventPublisher
{
    void PublishAsync<T>(T eventData) where T : class;
}