using DNDTracker.Domain;
using DNDTracker.Outbound.RabbitMq.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace DNDTracker.Outbound.RabbitMq;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the domain <see cref="IEventPublisher"/> port on top of NetPub. The NetPub
    /// RabbitMQ provider itself must be registered through the generated <c>AddNetPub</c>.
    /// </summary>
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services)
    {
        services.AddSingleton<IEventPublisher, NetPubEventPublisher>();

        return services;
    }
}