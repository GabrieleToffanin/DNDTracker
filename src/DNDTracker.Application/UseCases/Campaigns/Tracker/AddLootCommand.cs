using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record AddLootCommand(string CampaignName, LootResource Loot) : ICommand;
