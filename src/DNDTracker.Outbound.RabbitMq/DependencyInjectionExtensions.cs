using DNDTracker.Domain;
using DNDTracker.Outbound.RabbitMq.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace DNDTracker.Outbound.RabbitMq;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services)
    {
        services.AddSingleton<IEventPublisher, EventPublisher>();
        services.AddSingleton<IRabbitMqTopologyInitializer, RabbitMqTopologyInitializer>();
        
        return services;
    }
    
    public static async Task InitializeRabbitMqTopologyAsync(this IServiceProvider serviceProvider)
    {
        var topologyInitializer = serviceProvider.GetRequiredService<IRabbitMqTopologyInitializer>();
        await topologyInitializer.InitializeAsync();
    }
}