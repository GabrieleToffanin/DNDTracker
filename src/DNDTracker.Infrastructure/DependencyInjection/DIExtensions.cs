
using DNDTracker.BackendInfrastructure.PostgresDb.Repositories;
using DNDTracker.Domain.Campaigns;
using Microsoft.Extensions.DependencyInjection;

namespace DNDTracker.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering infrastructure services with the DI container.
/// </summary>
public static class DIExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICampaignRepository, PostgreCampaignRepository>();

        return services;
    }
}