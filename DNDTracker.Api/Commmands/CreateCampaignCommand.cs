using DNDTracker.Application.Responses;
using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Api.Commmands;

public record CreateCampaignCommand(
    string CampaignName,
    string CampaignDescription,
    string CampaignImage,
    bool IsActive = true) : ICommand<CampaignDto>;