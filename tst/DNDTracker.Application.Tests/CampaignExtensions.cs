using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Heroes;
using Force.DeepCloner;

namespace DNDTracker.Application.Tests;

internal static class CampaignExtensions
{
    internal static Campaign WithHeroes(this Campaign campaign, params Hero[] heroes)
    {
        var clonedCampaing = campaign.DeepClone();
        clonedCampaing.AddHero(heroes);
        return clonedCampaing;
    } 
}