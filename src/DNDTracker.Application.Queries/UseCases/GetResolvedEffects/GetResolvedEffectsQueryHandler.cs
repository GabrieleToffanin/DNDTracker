using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Services;
using DNDTracker.SharedKernel;
using DNDTracker.SharedKernel.Queries;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.Queries.UseCases.GetResolvedEffects;

public class GetResolvedEffectsQueryHandler(ICampaignRepository campaignRepository)
    : IQueryHandler<GetResolvedEffectsQuery, ResolvedEffects>
{
    public async Task<ResolvedEffects> Handle(GetResolvedEffectsQuery request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        var hero = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.HeroId)
            ?? throw new CharacterNotFoundException($"Hero {request.HeroId} not found.");

        return EffectResolver.ResolveEffectsForCombatant(hero);
    }
}
