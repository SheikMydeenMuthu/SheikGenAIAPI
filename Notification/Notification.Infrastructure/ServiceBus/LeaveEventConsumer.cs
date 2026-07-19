using System.Text.Json;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.DTOs;
using Notification.Application.Features.SendLeaveEmail;
using Microsoft.Extensions.DependencyInjection;

namespace Notification.Infrastructure.ServiceBus;

public class LeaveEventConsumer : BackgroundService
{
    private readonly ServiceBusSettings _settings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LeaveEventConsumer> _logger;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public LeaveEventConsumer(
        IOptions<ServiceBusSettings> settings,
        IServiceScopeFactory scopeFactory,
        ILogger<LeaveEventConsumer> logger)
    {
        _settings = settings.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client = new ServiceBusClient(_settings.ConnectionString);
        _processor = _client.CreateProcessor(_settings.TopicName, _settings.SubscriptionName,
            new ServiceBusProcessorOptions { MaxConcurrentCalls = 1, AutoCompleteMessages = false });

        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += HandleErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);
        _logger.LogInformation("LeaveEventConsumer started listening on {Topic}/{Subscription}", _settings.TopicName, _settings.SubscriptionName);

        await Task.Delay(Timeout.Infinite, stoppingToken).ContinueWith(_ => { });
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var json = args.Message.Body.ToString();
            var evt = JsonSerializer.Deserialize<LeaveRequestedEvent>(json, _jsonOptions);

            if (evt is not null)
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new SendLeaveEmailCommand { Event = evt });
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed processing message {MessageId}", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "ProcessingError", ex.Message);
        }
    }

    private Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Service Bus processor error");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null) await _processor.StopProcessingAsync(cancellationToken);
        if (_processor is not null) await _processor.DisposeAsync();
        if (_client is not null) await _client.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}