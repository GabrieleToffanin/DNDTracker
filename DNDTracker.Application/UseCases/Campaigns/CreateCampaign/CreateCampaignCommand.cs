using DNDTracker.Application.Responses;
using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.CreateCampaign;

public record CreateCampaignCommand(
    string CampaignName,
    string CampaignDescription,
    string CampaignImage,
    DateTime CreatedDate,
    bool IsActive = true) : ICommand;