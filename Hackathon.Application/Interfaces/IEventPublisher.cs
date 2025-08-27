namespace Hackathon.Application.Interfaces;

public interface IEventPublisher
{
    void PublishAsync<T>(T eventData) where T : class;
}