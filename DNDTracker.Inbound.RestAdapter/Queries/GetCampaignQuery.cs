using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Queries;

public class GetCampaignQuery
{
    [FromRoute] public required string CampaignName { get; set; }
}