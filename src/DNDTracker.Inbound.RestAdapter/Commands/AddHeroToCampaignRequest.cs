using DNDTracker.Domain.Heroes;
using DNDTracker.Inbound.RestAdapter.Dtos;

namespace DNDTracker.Inbound.RestAdapter.Commands;

public record AddHeroToCampaignRequest(
    HeroDto Hero);