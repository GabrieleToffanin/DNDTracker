using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.Application.UseCases.Campaigns.AddHero;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Inbound.RestAdapter.Commands;
using DNDTracker.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(
    IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CampaignDto>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        GetAllCampaigns getAllCampaigns = new();
        
        IEnumerable<CampaignDto> campaigns = await mediator.Send(getAllCampaigns, cancellationToken);
        
        return Ok(campaigns);
    }

    [HttpGet("{campaignName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CampaignDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        string campaignName,
        CancellationToken cancellationToken)
    {
        GetCampaignByName getByName = new(campaignName);
        
        CampaignDto campaign = await mediator.Send(getByName, cancellationToken);
        
        return Ok(campaign);
    }

    [HttpPost("{campaignName}/heroes")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(void))]
    public async Task<IActionResult> AddHero(
        string campaignName,
        [FromBody]AddHeroToCampaignRequest command,
        CancellationToken cancellationToken)
    {
        var mappedRequest = new AddHeroToCampaignCommand(
            campaignName,
            command.Hero);
        
        // send the command to the mediator to handle
        await mediator.Send(
            mappedRequest,
            cancellationToken);

        // if successful, return status code 201 (Created) with campaign data
        return CreatedAtAction(nameof(AddHero), new { campaignName }, null);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(void))]
    public async Task<IActionResult> CreateCampaign(
        [FromBody]CreateCampaignRequest command,
        CancellationToken cancellationToken)
    {
        var mappedRequest = new CreateCampaignCommand(
            command.CampaignName,
            command.CampaignDescription,
            command.CampaignImage,
            command.CreatedDate,
            command.IsActive);
        
        // send the command to the mediator to handle
        await mediator.Send(
            mappedRequest,
            cancellationToken);

        // if successful, return status code 201 (Created) with campaign data
        return CreatedAtAction(nameof(CreateCampaign), new { command.CampaignName }, null);
    }
}