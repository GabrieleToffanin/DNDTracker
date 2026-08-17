using DNDTracker.DataAccessObject.Mapping.CampaignMap;
using DNDTracker.DataAccessObject.Mapping.HeroMap;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Outbound.PostgresDb.Database.Postgres;
using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Outbound.PostgresDb.Repositories;

public class PostgreCampaignRepository(
    DNDTrackerPostgresDbContext context) : ICampaignRepository
{
    public async Task<Campaign?> GetCampaignAsync(string campaignName, CancellationToken cancellationToken)
    {
        var campaign = await context.Set<CampaignModel>()
            .IncludeTrackerGraph()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CampaignName == campaignName, cancellationToken);
        
        return campaign?.MapToDomain();
    }

    public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync(CancellationToken cancellationToken)
    {
        var campaigns = await context.Set<CampaignModel>()
            .IncludeTrackerGraph()
            .AsNoTracking()
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
                .IncludeTrackerGraph()
                .FirstOrDefaultAsync(c => c.CampaignName == campaign.CampaignName, cancellationToken);

            if (trackedModel is null)
                throw new InvalidOperationException($"Campaign '{campaign.CampaignName}' not found for update.");

            trackedModel.Apply(campaign.MapToModel());

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

}

internal static class CampaignModelQueryableExtensions
{
    public static IQueryable<CampaignModel> IncludeTrackerGraph(this IQueryable<CampaignModel> campaigns)
    {
        return campaigns
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.Inventory)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.Equipment)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.Spells)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.Spellbook)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.SpellSlots)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.Conditions)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.SavingThrowProficiencies)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.SkillProficiencies)
            .Include(campaign => campaign.Heroes)
                .ThenInclude(hero => hero.Feats)
            .Include(campaign => campaign.MonsterLibrary)
            .Include(campaign => campaign.ActiveCombat)
                .ThenInclude(combat => combat!.InitiativeOrder)
                    .ThenInclude(combatant => combatant.Conditions)
            .Include(campaign => campaign.SessionLogs)
            .Include(campaign => campaign.TimelineEntries)
            .Include(campaign => campaign.Npcs)
            .Include(campaign => campaign.Locations)
            .Include(campaign => campaign.Quests)
            .Include(campaign => campaign.Loot)
            .Include(campaign => campaign.Members)
            .AsSplitQuery();
    }
}