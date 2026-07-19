using System.Text.Json;
using Azure.Messaging.ServiceBus;
using HR.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace HR.Infrastructure.Messaging;

public class ServiceBusPublisher : IEventPublisher
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSettings _settings;

    public ServiceBusPublisher(IOptions<ServiceBusSettings> settings)
    {
        _settings = settings.Value;
        if (_settings.Enabled)
        {
            _client = new ServiceBusClient(_settings.ConnectionString);
        }
    }

    public async Task PublishAsync<T>(string topicName, T eventData, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled || _client is null)
        {
            // silently skip, or log
            return;
        }

        var sender = _client.CreateSender(topicName);
        var json = JsonSerializer.Serialize(eventData);
        var message = new ServiceBusMessage(json) { ContentType = "application/json" };
        await sender.SendMessageAsync(message, cancellationToken);
        await sender.CloseAsync(cancellationToken);
    }
}