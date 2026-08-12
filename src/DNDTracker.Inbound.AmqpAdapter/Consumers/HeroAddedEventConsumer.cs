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
    private IModel? _channel;
    private IConnection? _connection;
    private EventingBasicConsumer? _consumer;
    private const string QueueName = "dndtracking.campaign.hero-added";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _connection = CreateConnection();
            _channel = _connection.CreateModel();

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (_, ea) =>
            {
                try
                {
                    ProcessMessage(ea);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing hero-added event");
                }
            };

            _channel.BasicConsume(QueueName, autoAck: false, consumer: _consumer);
            logger.LogInformation("✅ HeroAddedEventConsumer started - listening on {QueueName}", QueueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("🟡 HeroAddedEventConsumer cancelled");
        }
    }

    private IConnection CreateConnection()
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

        return factory.CreateConnection();
    }

    private void ProcessMessage(BasicDeliverEventArgs ea)
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
            _channel!.BasicAck(ea.DeliveryTag, false);
            return;
        }

        activity?.SetTag("messaging.message_id", message.Id.ToString());

        logger.LogInformation(
            "🦸 Received hero-added event: Id={EventId}, OccuredOn={OccuredOn}",
            message.Id, message.OccuredOn);

        _channel!.BasicAck(ea.DeliveryTag, false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("🛑 HeroAddedEventConsumer stopping");

        await base.StopAsync(cancellationToken);

        if (_channel != null)
        {
            _channel.Close();
            _channel.Dispose();
            _channel = null;
        }

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync(CancellationToken.None);
    }
}
