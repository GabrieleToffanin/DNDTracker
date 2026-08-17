using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record AddMonsterToLibraryCommand(string CampaignName, MonsterStatBlock Monster) : ICommand;
