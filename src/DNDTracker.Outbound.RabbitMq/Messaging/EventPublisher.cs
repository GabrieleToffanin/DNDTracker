using System.Diagnostics;
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
        cancellationToken.ThrowIfCancellationRequested();

        await using IConnection connection = await CreateConnectionAsync(cancellationToken);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        byte[] body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
        string messageType = message.GetType().Name;

        string queueName = rabbitConfiguration.Value.Topology.Queues[messageType].Name;

        string exchange = GetExchangeForMessageType(queueName);
        string routingKey = GetRoutingKeyForMessageType(queueName);

        using var activity = RabbitMqTelemetry.ActivitySource.StartActivity(
            $"{exchange} publish",
            ActivityKind.Producer);

        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", exchange);
        activity?.SetTag("messaging.destination_kind", "exchange");
        activity?.SetTag("messaging.rabbitmq.routing_key", routingKey);
        activity?.SetTag("messaging.message_type", messageType);
        activity?.SetTag("server.address", rabbitConfiguration.Value.Host);
        activity?.SetTag("server.port", rabbitConfiguration.Value.Port);
        activity?.SetTag("peer.service", "rabbitmq");

        Dictionary<string, object?> headers = new();
        RabbitMqTelemetry.InjectTraceContext(activity, headers);

        BasicProperties basicProperties = new()
        {
            Headers = headers
        };

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: basicProperties,
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
}
