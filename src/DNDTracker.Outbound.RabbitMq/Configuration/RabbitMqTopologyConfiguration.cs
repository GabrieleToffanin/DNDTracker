namespace DNDTracker.Outbound.RabbitMq.Configuration;

public class RabbitMqTopologyConfiguration
{
    public Dictionary<string, ExchangeConfiguration> Exchanges { get; set; } = new();
    public Dictionary<string, QueueConfiguration> Queues { get; set; } = new();
    public List<BindingConfiguration> Bindings { get; set; } = new();
}