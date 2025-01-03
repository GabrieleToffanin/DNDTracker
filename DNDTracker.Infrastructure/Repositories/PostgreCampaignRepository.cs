using DNDTracker.Domain.Abstractions;
using DNDTracker.Domain.Entities;
using DNDTracker.Infrastructure.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Infrastructure.Repositories;

public class PostgreCampaignRepository(
    DNDTrackerPostgresDbContext context) : ICampaignRepository
{
    public async Task<Campaign?> GetCampaignAsync(string campaignName)
    {
        var connectionString = context.Database.GetDbConnection().ConnectionString;
        
        // Just a placeholder for making the test pass and fail
        return await context.Set<Campaign>()
            .FirstOrDefaultAsync(c => c.CampaignName == campaignName);
    }
}