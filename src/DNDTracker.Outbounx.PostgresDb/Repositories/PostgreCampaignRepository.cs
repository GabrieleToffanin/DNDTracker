using DNDTracker.DataAccessObject.Mapping.CampaignMap;
using DNDTracker.DataAccessObject.Mapping.HeroMap;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Outbounx.PostgresDb.Database.Postgres;
using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Outbounx.PostgresDb.Repositories;

public class PostgreCampaignRepository(
    DNDTrackerPostgresDbContext context) : ICampaignRepository
{
    public async Task<Campaign?> GetCampaignAsync(string campaignName, CancellationToken cancellationToken)
    {
        var campaign = await context.Set<CampaignModel>()
            .Include(c => c.Heroes)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CampaignName == campaignName, cancellationToken);
        
        return campaign?.MapToDomain();
    }

    public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync(CancellationToken cancellationToken)
    {
        var campaigns = await context.Set<CampaignModel>()
            .ToListAsync(cancellationToken);
        
        return campaigns.Select(c => c.MapToDomain());
    }

    public async Task CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        var campaignModel = campaign.MapToModel();
        
        await context.Set<CampaignModel>()
            .AddAsync(campaignModel, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private const int MaxConcurrencyRetries = 3;

    public async Task UpdateAsync(
        Campaign campaign,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; ; attempt++)
        {
            var trackedModel = await context.Set<CampaignModel>()
                .Include(c => c.Heroes)
                .FirstOrDefaultAsync(c => c.CampaignName == campaign.CampaignName, cancellationToken);

            if (trackedModel is null)
                throw new InvalidOperationException($"Campaign '{campaign.CampaignName}' not found for update.");

            UpdateTrackedModel(trackedModel, campaign);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return;
            }
            catch (DbUpdateConcurrencyException) when (attempt < MaxConcurrencyRetries)
            {
                // The data was modified/removed concurrently (e.g. duplicate/retried request
                // already applied the same change). Detach the affected entries so the next
                // attempt reloads fresh state from the database and retries the update.
                foreach (var entry in context.ChangeTracker.Entries().ToList())
                    entry.State = EntityState.Detached;
            }
        }
    }

    private void UpdateTrackedModel(CampaignModel trackedModel, Campaign campaign)
    {
        trackedModel.CampaignName = campaign.CampaignName;
        trackedModel.CampaignDescription = campaign.CampaignDescription;
        trackedModel.CampaignImage = campaign.CampaignImage;
        trackedModel.IsActive = campaign.IsActive;
        trackedModel.UpdatedDate = campaign.UpdatedDate;

        HashSet<Guid> existingIds = trackedModel.Heroes.Select(h => h.Id).ToHashSet();
        var newHeroes = campaign.Heroes.Where(h => !existingIds.Contains(h.Id.Id));

        foreach (var hero in newHeroes)
            trackedModel.Heroes.Add(hero.MapToModel());
    }
}