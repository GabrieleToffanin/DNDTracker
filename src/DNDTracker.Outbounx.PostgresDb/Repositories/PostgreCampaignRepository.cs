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

    public async Task UpdateAsync(
        Campaign campaign,
        CancellationToken cancellationToken)
    {
        var trackedModel = context.ChangeTracker.Entries<CampaignModel>()
            .First(model => model.Entity.Id == campaign.Id.Id).Entity;
        
        UpdateTrackedModel(trackedModel, campaign);
        
        await context.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateTrackedModel(
        CampaignModel trackedModel,
        Campaign campaign)
    {
        trackedModel.CampaignName = campaign.CampaignName;
        trackedModel.CampaignDescription = campaign.CampaignDescription;
        trackedModel.CampaignImage = campaign.CampaignImage;
        trackedModel.IsActive = campaign.IsActive;
        trackedModel.Heroes.Clear();
        
        trackedModel.Heroes.AddRange(campaign.Heroes.Select(h => h.MapToModel()));
    }
}