namespace Hackathon.Abstractions.Interfaces;

/// <summary>
/// Interface para publicação de eventos
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publica um evento de forma síncrona
    /// </summary>
    /// <typeparam name="TEvent">Tipo do evento</typeparam>
    /// <param name="event">Evento a ser publicado</param>
    void Publish<TEvent>(TEvent @event) where TEvent : class;
    
    /// <summary>
    /// Publica um evento de forma assíncrona
    /// </summary>
    /// <typeparam name="TEvent">Tipo do evento</typeparam>
    /// <param name="event">Evento a ser publicado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
    
    /// <summary>
    /// Publica um evento como string de forma síncrona
    /// </summary>
    /// <param name="eventData">Dados do evento como string</param>
    void Publish(string eventData);
    
    /// <summary>
    /// Publica um evento como string de forma assíncrona
    /// </summary>
    /// <param name="eventData">Dados do evento como string</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task PublishAsync(string eventData, CancellationToken cancellationToken = default);
}
