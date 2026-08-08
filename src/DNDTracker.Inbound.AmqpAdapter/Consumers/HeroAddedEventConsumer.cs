using System.Text.Json;
using DNDTracker.Domain.Campaigns.DomainEvents;
using DNDTracker.Outbound.RabbitMq.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DNDTracker.Inbound.AmqpAdapter.Consumers;

public class HeroAddedEventConsumer(
    IOptions<RabbitMqConfiguration> rabbitConfiguration,
    ILogger<HeroAddedEventConsumer> logger)
{
    private IChannel? _channel;
    private IConnection? _connection;
    private const string QueueName = "dndtracking.campaign.hero-added";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                await ProcessMessageAsync(ea, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing hero-added event");
            }
        };

        await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
        logger.LogInformation("✅ HeroAddedEventConsumer started - listening on {QueueName}", QueueName);
    }

    private async Task<IConnection> CreateConnectionAsync()
    {
        var factory = new ConnectionFactory
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

        return await factory.CreateConnectionAsync();
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        var body = ea.Body.ToArray();
        var message = JsonSerializer.Deserialize<HeroAddedDomainEvent>(body);

        if (message == null)
        {
            logger.LogWarning("Failed to deserialize HeroAddedDomainEvent");
            await _channel!.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            return;
        }

        logger.LogInformation(
            "🦸 Received hero-added event: Id={EventId}, OccuredOn={OccuredOn}",
            message.Id, message.OccuredOn);

        await _channel!.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }
        
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
