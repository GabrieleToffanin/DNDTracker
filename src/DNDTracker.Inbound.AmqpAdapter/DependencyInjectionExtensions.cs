using DNDTracker.Inbound.AmqpAdapter.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace DNDTracker.Inbound.AmqpAdapter;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddAmqpAdapter(this IServiceCollection services)
    {
        services.AddHostedService<HeroAddedEventConsumer>();

        return services;
    }
}
