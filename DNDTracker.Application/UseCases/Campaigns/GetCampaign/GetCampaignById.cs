using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.UseCases.Campaigns.GetCampaign;

public record GetCampaignById(Guid CampaignId) : IQuery<Campaign>;