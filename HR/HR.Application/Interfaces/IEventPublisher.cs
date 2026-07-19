namespace HR.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topicName, T eventData, CancellationToken cancellationToken = default);
}