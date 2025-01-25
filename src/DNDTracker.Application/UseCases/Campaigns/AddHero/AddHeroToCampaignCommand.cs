using DNDTracker.Domain.Heroes;
using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.AddHero;

public record AddHeroToCampaignCommand(string CampaignName, Hero Hero) : ICommand;