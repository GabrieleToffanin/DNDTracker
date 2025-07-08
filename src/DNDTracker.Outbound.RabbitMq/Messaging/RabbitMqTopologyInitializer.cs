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
        var channel = await GetChannelAsync();
        
        try
        {
            await DeclareExchangesAsync(channel, cancellationToken);
            await DeclareQueuesAsync(channel, cancellationToken);
            await CreateBindingsAsync(channel, cancellationToken);
            
            logger.LogInformation("RabbitMQ topology initialized successfully");
        }
        finally
        {
            await channel.CloseAsync();
        }
    }

    private async Task DeclareExchangesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        var exchanges = rabbitConfiguration.Value.Topology.Exchanges;
        
        foreach (var (key, exchange) in exchanges)
        {
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
        var queues = rabbitConfiguration.Value.Topology.Queues;
        
        foreach (var (key, queue) in queues)
        {
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
        var bindings = rabbitConfiguration.Value.Topology.Bindings;
        
        foreach (var binding in bindings)
        {
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

    private async Task<IChannel> GetChannelAsync()
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