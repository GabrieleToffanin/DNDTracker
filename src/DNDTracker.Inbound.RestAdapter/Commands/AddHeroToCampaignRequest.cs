using DNDTracker.Domain.Heroes;

namespace DNDTracker.Inbound.RestAdapter.Commands;

public record AddHeroToCampaignRequest(
    Hero Hero);