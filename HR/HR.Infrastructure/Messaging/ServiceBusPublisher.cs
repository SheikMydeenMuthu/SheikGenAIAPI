using System.Text.Json;
using Azure.Messaging.ServiceBus;
using HR.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace HR.Infrastructure.Messaging;

public class ServiceBusPublisher : IEventPublisher
{
    private readonly ServiceBusClient _client;

    public ServiceBusPublisher(IOptions<ServiceBusSettings> settings)
    {
        _client = new ServiceBusClient(settings.Value.ConnectionString);
    }

    public async Task PublishAsync<T>(string topicName, T eventData, CancellationToken cancellationToken = default)
    {
        var sender = _client.CreateSender(topicName);
        var json = JsonSerializer.Serialize(eventData);
        var message = new ServiceBusMessage(json) { ContentType = "application/json" };
        await sender.SendMessageAsync(message, cancellationToken);
        await sender.CloseAsync(cancellationToken);
    }
}