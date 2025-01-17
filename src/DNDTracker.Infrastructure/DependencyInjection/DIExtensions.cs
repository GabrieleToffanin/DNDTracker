using DNDTracker.BackendInfrastructure.PostgresDb.Repositories;
using DNDTracker.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DNDTracker.Infrastructure.DependencyInjection;

/// <summary>
/// The database registration should go there,
/// at the moment I'll keep it in the Program.cs
/// </summary>
public static class DIExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICampaignRepository, PostgreCampaignRepository>();

        return services;
    }
}