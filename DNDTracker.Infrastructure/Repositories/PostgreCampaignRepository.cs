using DNDTracker.Domain.Abstractions;
using DNDTracker.Domain.Entities;
using DNDTracker.Infrastructure.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Infrastructure.Repositories;

public class PostgreCampaignRepository(
    DNDTrackerPostgresDbContext context) : ICampaignRepository
{
    public async Task<Campaign?> GetCampaignAsync(string campaignName, CancellationToken cancellationToken)
    {
        return await context.Set<Campaign>()
            .FirstOrDefaultAsync(c => c.CampaignName == campaignName, cancellationToken);
    }

    public async Task CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        await context.Set<Campaign>().AddAsync(campaign, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}