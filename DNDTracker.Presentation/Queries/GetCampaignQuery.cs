using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Presentation.Queries;

public class GetCampaignQuery
{
    [FromRoute] public Guid CampaignId { get; set; }
}