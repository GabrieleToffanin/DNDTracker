using DNDTracker.Outbound.RabbitMq.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DNDTracker.Outbound.RabbitMq.Messaging;

internal interface IRabbitMqTopologyInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

internal class RabbitMqTopologyInitializer(
    IOptions<RabbitMqConfiguration> rabbitConfiguration,
    ILogger<RabbitMqTopologyInitializer> logger) : IRabbitMqTopologyInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using IConnection connection = await CreateConnectionAsync(cancellationToken);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        try
        {
            await DeclareExchangesAsync(channel, cancellationToken);
            await DeclareQueuesAsync(channel, cancellationToken);
            await CreateBindingsAsync(channel, cancellationToken);

            logger.LogInformation("RabbitMQ topology initialized successfully");
        }
        finally
        {
            await channel.CloseAsync(CancellationToken.None);
        }
    }

    private async Task DeclareExchangesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var (_, exchange) in rabbitConfiguration.Value.Topology.Exchanges)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await channel.ExchangeDeclareAsync(
                exchange: exchange.Name,
                type: exchange.Type,
                durable: exchange.Durable,
                autoDelete: exchange.AutoDelete,
                arguments: exchange.Arguments,
                cancellationToken: cancellationToken);

            logger.LogDebug("Declared exchange: {ExchangeName} of type {ExchangeType}",
                exchange.Name, exchange.Type);
        }
    }

    private async Task DeclareQueuesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var (_, queue) in rabbitConfiguration.Value.Topology.Queues)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await channel.QueueDeclareAsync(
                queue: queue.Name,
                durable: queue.Durable,
                exclusive: queue.Exclusive,
                autoDelete: queue.AutoDelete,
                arguments: queue.Arguments,
                cancellationToken: cancellationToken);

            logger.LogDebug("Declared queue: {QueueName}", queue.Name);
        }
    }

    private async Task CreateBindingsAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var binding in rabbitConfiguration.Value.Topology.Bindings)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await channel.QueueBindAsync(
                queue: binding.Queue,
                exchange: binding.Exchange,
                routingKey: binding.RoutingKey,
                arguments: binding.Arguments,
                cancellationToken: cancellationToken);

            logger.LogDebug("Created binding: {Queue} -> {Exchange} with routing key {RoutingKey}",
                binding.Queue, binding.Exchange, binding.RoutingKey);
        }
    }

    private Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
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

        return factory.CreateConnectionAsync(cancellationToken);
    }
}