using RabbitMQ.Client;

namespace DNDTracker.Outbound.RabbitMq.Configuration;

/// <summary>
/// Broker connection settings bound from the <c>RabbitMQ</c> configuration section.
/// Topology (exchanges, queues, bindings) is declared in code through NetPub attributes.
/// </summary>
public class RabbitMqConfiguration
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public int ConnectionTimeout { get; set; } = 30000;
    public int RequestedHeartbeat { get; set; } = 120;

    public IConnectionFactory CreateConnectionFactory() => new ConnectionFactory
    {
        HostName = Host,
        Port = Port,
        UserName = Username,
        Password = Password,
        VirtualHost = VirtualHost,
        RequestedConnectionTimeout = TimeSpan.FromMilliseconds(ConnectionTimeout),
        RequestedHeartbeat = TimeSpan.FromSeconds(RequestedHeartbeat),
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        AutomaticRecoveryEnabled = true
    };
}