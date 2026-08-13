using System.Diagnostics;
using System.Text.Json;
using DNDTracker.Domain.Campaigns.DomainEvents;
using DNDTracker.Outbound.RabbitMq.Configuration;
using DNDTracker.Outbound.RabbitMq.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DNDTracker.Inbound.AmqpAdapter.Consumers;

public class HeroAddedEventConsumer(
    IOptions<RabbitMqConfiguration> rabbitConfiguration,
    ILogger<HeroAddedEventConsumer> logger) : BackgroundService, IAsyncDisposable
{
    private IChannel? _channel;
    private IConnection? _connection;
    private const string QueueName = "dndtracking.campaign.hero-added";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _connection = await CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    await ProcessMessageAsync(ea, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing hero-added event");
                }
            };

            await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
            logger.LogInformation("✅ HeroAddedEventConsumer started - listening on {QueueName}", QueueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("🟡 HeroAddedEventConsumer cancelled");
        }
    }

    private Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new()
        {
            HostName = rabbitConfiguration.Value.Host,
            Port = rabbitConfiguration.Value.Port,
            UserName = rabbitConfiguration.Value.Username,
            Password = rabbitConfiguration.Value.Password,
            VirtualHost = rabbitConfiguration.Value.VirtualHost,
            RequestedHeartbeat = TimeSpan.FromSeconds(rabbitConfiguration.Value.RequestedHeartbeat),
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            AutomaticRecoveryEnabled = true
        };

        return factory.CreateConnectionAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        var (traceParent, traceState) = RabbitMqTelemetry.ExtractTraceContext(ea.BasicProperties?.Headers);
        ActivityContext.TryParse(traceParent, traceState, out var parentContext);

        using var activity = RabbitMqTelemetry.ActivitySource.StartActivity(
            $"{QueueName} process",
            ActivityKind.Consumer,
            parentContext);

        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", QueueName);
        activity?.SetTag("messaging.operation", "process");

        byte[] body = ea.Body.ToArray();
        HeroAddedDomainEvent? message = JsonSerializer.Deserialize<HeroAddedDomainEvent>(body);

        if (message == null)
        {
            logger.LogWarning("Failed to deserialize HeroAddedDomainEvent");
            activity?.SetStatus(ActivityStatusCode.Error, "Deserialization failed");
            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            return;
        }

        activity?.SetTag("messaging.message_id", message.Id.ToString());

        logger.LogInformation(
            "🦸 Received hero-added event: Id={EventId}, OccuredOn={OccuredOn}",
            message.Id, message.OccuredOn);

        await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("🛑 HeroAddedEventConsumer stopping");

        await base.StopAsync(cancellationToken);

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync(CancellationToken.None);
    }
}
