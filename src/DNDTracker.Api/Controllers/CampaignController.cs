using DNDTracker.Api.Queries;
using DNDTracker.Application.Responses;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using DNDTracker.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(
    IMediator mediator) : ControllerBase
{
    [HttpGet("{campaignName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CampaignDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        GetCampaignQuery query,
        CancellationToken cancellationToken)
    {
        // Search for the specific campaign based on received guid
        GetCampaignByName getByName = new(query.CampaignName);
        
        CampaignDto campaign = await mediator.Send(getByName, cancellationToken);
        
        return Ok(campaign);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(void))]
    public async Task<IActionResult> Create(
        [FromBody]CreateCampaignCommand command,
        CancellationToken cancellationToken)
    {
        // send the command to the mediator to handle
        await mediator.Send(command, cancellationToken);

        // if successful, return status code 201 (Created) with campaign data
        return Created();
    }
    
}