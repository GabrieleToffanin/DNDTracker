using DNDTracker.Domain;
using DNDTracker.Outbound.RabbitMq.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DNDTracker.Outbound.RabbitMq.Messaging;

internal class EventPublisher(
    IOptions<RabbitMqConfiguration> rabbitConfiguration) : IEventPublisher
{
    public async ValueTask PublishAsync<T>(
    T message,
    CancellationToken cancellationToken = default)
    where T : notnull
    {
        var channel = await GetConnection();
        
        var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
        var messageType = message.GetType().Name;
        
        var queueName = rabbitConfiguration.Value.Topology.Queues[messageType].Name;
        
        var exchange = GetExchangeForMessageType(queueName);
        var routingKey = GetRoutingKeyForMessageType(queueName);

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            body: body,
            cancellationToken: cancellationToken);
    }


    private string GetExchangeForMessageType(string messageType)
    {
        var binding = rabbitConfiguration.Value.Topology.Bindings
            .FirstOrDefault(b => b.Queue.Equals(messageType, StringComparison.OrdinalIgnoreCase));
            
        return binding?.Exchange ?? "";
    }

    private string GetRoutingKeyForMessageType(string messageType)
    {
        var binding = rabbitConfiguration.Value.Topology.Bindings
            .FirstOrDefault(b => b.Queue.Equals(messageType, StringComparison.OrdinalIgnoreCase));
            
        return binding?.RoutingKey ?? messageType;
    }

    private async Task<IChannel> GetConnection()
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

        var connection = await factory.CreateConnectionAsync();
        return await connection.CreateChannelAsync();
    }
}


