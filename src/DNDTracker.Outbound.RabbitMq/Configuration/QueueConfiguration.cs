namespace DNDTracker.Outbound.RabbitMq.Configuration;

public class QueueConfiguration
{
    public string Name { get; set; }
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public Dictionary<string, object> Arguments { get; set; } = new();
}