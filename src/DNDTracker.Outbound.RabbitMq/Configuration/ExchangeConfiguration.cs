namespace DNDTracker.Outbound.RabbitMq.Configuration;

public class ExchangeConfiguration
{
    public string Name { get; set; }
    public string Type { get; set; } = "topic";
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; } = false;
    public Dictionary<string, object> Arguments { get; set; } = new();
}