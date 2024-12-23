using DNDTracker.Presentation.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    [HttpGet("{campaignId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(GetCampaignQuery query)
    {
        // Search for the specific campaign based on received guid
        return Ok("Campaign");
    }
}