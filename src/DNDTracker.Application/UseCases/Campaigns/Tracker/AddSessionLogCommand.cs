using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record AddSessionLogCommand(string CampaignName, SessionLogEntry SessionLog) : ICommand;
