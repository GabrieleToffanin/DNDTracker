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
    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using IConnection connection = CreateConnection();
        using IModel channel = connection.CreateModel();
        
        try
        {
            DeclareExchanges(channel, cancellationToken);
            DeclareQueues(channel, cancellationToken);
            CreateBindings(channel, cancellationToken);
            
            logger.LogInformation("RabbitMQ topology initialized successfully");
        }
        finally
        {
            channel.Close();
        }

        return Task.CompletedTask;
    }

    private void DeclareExchanges(IModel channel, CancellationToken cancellationToken)
    {
        var exchanges = rabbitConfiguration.Value.Topology.Exchanges;
        
        foreach (var (key, exchange) in exchanges)
        {
            cancellationToken.ThrowIfCancellationRequested();

            channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Type,
                durable: exchange.Durable,
                autoDelete: exchange.AutoDelete,
                arguments: exchange.Arguments);
                
            logger.LogDebug("Declared exchange: {ExchangeName} of type {ExchangeType}", 
                exchange.Name, exchange.Type);
        }
    }

    private void DeclareQueues(IModel channel, CancellationToken cancellationToken)
    {
        var queues = rabbitConfiguration.Value.Topology.Queues;
        
        foreach (var (key, queue) in queues)
        {
            cancellationToken.ThrowIfCancellationRequested();

            channel.QueueDeclare(
                queue: queue.Name,
                durable: queue.Durable,
                exclusive: queue.Exclusive,
                autoDelete: queue.AutoDelete,
                arguments: queue.Arguments);
                
            logger.LogDebug("Declared queue: {QueueName}", queue.Name);
        }
    }

    private void CreateBindings(IModel channel, CancellationToken cancellationToken)
    {
        var bindings = rabbitConfiguration.Value.Topology.Bindings;
        
        foreach (var binding in bindings)
        {
            cancellationToken.ThrowIfCancellationRequested();

            channel.QueueBind(
                queue: binding.Queue,
                exchange: binding.Exchange,
                routingKey: binding.RoutingKey,
                arguments: binding.Arguments);
                
            logger.LogDebug("Created binding: {Queue} -> {Exchange} with routing key {RoutingKey}", 
                binding.Queue, binding.Exchange, binding.RoutingKey);
        }
    }

    private IConnection CreateConnection()
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

        return factory.CreateConnection();
    }
}