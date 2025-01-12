using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Api.Queries;

public class GetCampaignQuery
{
    [FromRoute] public required string CampaignName { get; set; }
}