using DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres;
using DNDTracker.BackendInfrastructure.PostgresDb.Models;
using DNDTracker.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.BackendInfrastructure.PostgresDb.Repositories;

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
        await context.Set<Campaign>()
            .AddAsync(campaign, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}