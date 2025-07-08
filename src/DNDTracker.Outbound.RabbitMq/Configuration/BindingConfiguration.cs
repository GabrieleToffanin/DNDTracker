namespace DNDTracker.Outbound.RabbitMq.Configuration;

public class BindingConfiguration
{
    public string Exchange { get; set; }
    public string Queue { get; set; }
    public string RoutingKey { get; set; }
    public Dictionary<string, object> Arguments { get; set; } = new();
}