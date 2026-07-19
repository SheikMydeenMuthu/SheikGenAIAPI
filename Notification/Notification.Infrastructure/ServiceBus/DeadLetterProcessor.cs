using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Notification.Infrastructure.ServiceBus;

public class DeadLetterProcessor
{
    private readonly ServiceBusSettings _settings;
    private readonly ILogger<DeadLetterProcessor> _logger;

    public DeadLetterProcessor(IOptions<ServiceBusSettings> settings, ILogger<DeadLetterProcessor> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task ReadAndClearAsync(CancellationToken cancellationToken = default)
    {
        await using var client = new ServiceBusClient(_settings.ConnectionString);

        // DLQ path convention: {topic}/Subscriptions/{subscription}/$DeadLetterQueue
        var receiver = client.CreateReceiver(
            _settings.TopicName,
            _settings.SubscriptionName,
            new ServiceBusReceiverOptions { SubQueue = SubQueue.DeadLetter });

        while (true)
        {
            var messages = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken);
            if (messages.Count == 0) break;

            foreach (var msg in messages)
            {
                _logger.LogWarning(
                    "DeadLetter Msg: {Body} | Reason: {Reason} | Description: {Description}",
                    msg.Body.ToString(),
                    msg.DeadLetterReason,
                    msg.DeadLetterErrorDescription);

                // Complete = permanently remove from DLQ
                await receiver.CompleteMessageAsync(msg, cancellationToken);
            }
        }

        await receiver.CloseAsync(cancellationToken);
        _logger.LogInformation("Dead-letter queue cleared.");
    }
}