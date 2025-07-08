namespace DNDTracker.Outbound.RabbitMq.Configuration;

public class RabbitMqConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
    public int ConnectionTimeout { get; set; }
    public int RequestedHeartbeat { get; set; }
    
    public RabbitMqTopologyConfiguration Topology { get; set; } = new();
}