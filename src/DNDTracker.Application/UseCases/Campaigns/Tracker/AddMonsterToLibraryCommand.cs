using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record AddMonsterToLibraryCommand(string CampaignName, MonsterStatBlock Monster) : ICommand;
