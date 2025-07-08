namespace DNDTracker.Outbound.InMemoryAdapter.Messaging;

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

public class RabbitMqTopologyConfiguration
{
    public Dictionary<string, ExchangeConfiguration> Exchanges { get; set; } = new();
    public Dictionary<string, QueueConfiguration> Queues { get; set; } = new();
    public List<BindingConfiguration> Bindings { get; set; } = new();
}

public class ExchangeConfiguration
{
    public string Name { get; set; }
    public string Type { get; set; } = "topic";
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; } = false;
    public Dictionary<string, object> Arguments { get; set; } = new();
}

public class QueueConfiguration
{
    public string Name { get; set; }
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public Dictionary<string, object> Arguments { get; set; } = new();
}

public class BindingConfiguration
{
    public string Exchange { get; set; }
    public string Queue { get; set; }
    public string RoutingKey { get; set; }
    public Dictionary<string, object> Arguments { get; set; } = new();
}