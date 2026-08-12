using System.Diagnostics;
using DNDTracker.Domain;
using DNDTracker.Outbound.RabbitMq.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DNDTracker.Outbound.RabbitMq.Messaging;

internal class EventPublisher(
    IOptions<RabbitMqConfiguration> rabbitConfiguration) : IEventPublisher
{
    public ValueTask PublishAsync<T>(
    T message,
    CancellationToken cancellationToken = default)
    where T : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();

        using IConnection connection = CreateConnection();
        using IModel channel = connection.CreateModel();

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

        Dictionary<string, object?> headers = new();
        RabbitMqTelemetry.InjectTraceContext(activity, headers);

        IBasicProperties basicProperties = channel.CreateBasicProperties();
        basicProperties.Headers = headers;

        channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: basicProperties,
            body: body);

        return ValueTask.CompletedTask;
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
}
