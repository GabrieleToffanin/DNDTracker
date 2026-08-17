using DNDTracker.SharedKernel;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.Queries.UseCases.GetResolvedEffects;

public record GetResolvedEffectsQuery(string CampaignName, Guid HeroId) : IQuery<ResolvedEffects>;
