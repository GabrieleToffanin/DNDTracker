using System.Net;
using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.Application.UseCases.Campaigns.AddHero;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Domain.Heroes;
using DNDTracker.Inbound.RestAdapter.Commands;
using DNDTracker.Inbound.RestAdapter.Controllers;
using DNDTracker.Inbound.RestAdapter.Dtos;
using DNDTracker.Inbound.RestAdapter.Queries;
using DNDTracker.InMemory.Adapter;
using DNDTracker.SharedKernel;
using DNDTracker.Vocabulary.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Tests;

public class CampaignControllerTests
{
    [Fact]
    public async Task Get_ReturnsOkResult_WhenCampaignExists()
    {
        // Arrange
        var mediator = new DummyMediator();
        var expectedCampaign = new CampaignDto(
            "Test Campaign",
            "Description");
        
        mediator.RegisterHandler<GetCampaignByName, CampaignDto>((request, _) => 
            expectedCampaign);
        
        var controller = new CampaignController(mediator);
        var query = new GetCampaignQuery
        {
            CampaignName = "Test Campaign"
        };

        // Act
        var result = await controller.Get(query.CampaignName, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedCampaign);
    }

    [Fact]
    public async Task AddHero_ReturnsCreatedResult_WhenSuccessful()
    {
        // Arrange
        var mediator = new DummyMediator();
        var campaignName = "Test Campaign";
        var heroRequest = new HeroDto(
            "Aragorn",
            HeroClass.Fighter, 
            Race.Human, 
            Alignment.Lawful | Alignment.Good,
            10, 
            1000, 
            50, 
            DiceType.D8);
        
        mediator.RegisterHandler<AddHeroToCampaignCommand>((_, __) => Task.CompletedTask);
        
        var controller = new CampaignController(mediator);
        var request = new AddHeroToCampaignRequest (heroRequest );

        // Act
        var result = await controller.AddHero(campaignName, request, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        createdResult.ActionName.Should().Be(nameof(CampaignController.AddHero));
        createdResult.RouteValues.Should().ContainKey("campaignName")
            .WhoseValue.Should().Be(campaignName);
    }

    [Fact]
    public async Task CreateCampaign_ReturnsCreatedResult_WhenSuccessful()
    {
        // Arrange
        var mediator = new DummyMediator();
        var campaignRequest = new CreateCampaignRequest(
            "New Campaign",
            "Test description",
            "image.jpg",
            DateTime.Now,
            true
        );
        
        mediator.RegisterHandler<CreateCampaignCommand>((_, __) => Task.CompletedTask);
        
        var controller = new CampaignController(mediator);

        // Act
        var result = await controller.CreateCampaign(campaignRequest, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        createdResult.ActionName.Should().Be(nameof(CampaignController.CreateCampaign));
        createdResult.RouteValues.Should().ContainKey("campaignName")
            .WhoseValue.Should().Be(campaignRequest.CampaignName);
    }
}