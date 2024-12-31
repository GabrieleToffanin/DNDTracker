using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.UseCases.Campaigns.GetCampaign;

public record GetCampaignByName(string CampaignName) : IQuery<Campaign>;