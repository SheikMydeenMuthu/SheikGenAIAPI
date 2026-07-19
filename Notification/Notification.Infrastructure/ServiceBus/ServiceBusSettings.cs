namespace Notification.Infrastructure.ServiceBus;

public class ServiceBusSettings
{
    public string ConnectionString { get; set; } = default!;
    public string TopicName { get; set; } = default!;
    public string SubscriptionName { get; set; } = default!;
}