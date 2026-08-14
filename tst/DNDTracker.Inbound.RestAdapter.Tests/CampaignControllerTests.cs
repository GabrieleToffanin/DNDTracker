using System.Net;
using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.Application.Queries.UseCases.GetCampaignTracker;
using DNDTracker.Application.Queries.UseCases.RollDice;
using DNDTracker.Application.UseCases.Campaigns.AddHero;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Inbound.RestAdapter.Commands;
using DNDTracker.Inbound.RestAdapter.Controllers;
using DNDTracker.Inbound.RestAdapter.Dtos;
using DNDTracker.InMemory.Adapter;
using DNDTracker.SharedKernel;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Tests;

public class CampaignControllerTests
{
    [Fact]
    public async Task Get_ReturnsOkResult_WhenCampaignExists()
    {
        var mediator = new DummyMediator();
        var expectedCampaign = new CampaignDto(
            "Test Campaign",
            "Description");

        mediator.RegisterHandler<GetCampaignByName, CampaignDto>((request, _) =>
            expectedCampaign);

        var controller = new CampaignController(mediator);

        var result = await controller.Get("Test Campaign", CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedCampaign);
    }

    [Fact]
    public async Task AddHero_ReturnsCreatedResult_WhenSuccessful()
    {
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
        var request = new AddHeroToCampaignRequest(heroRequest);

        var result = await controller.AddHero(campaignName, request, CancellationToken.None);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        createdResult.ActionName.Should().Be(nameof(CampaignController.AddHero));
        createdResult.RouteValues.Should().ContainKey("campaignName")
            .WhoseValue.Should().Be(campaignName);
    }

    [Fact]
    public async Task CreateCampaign_ReturnsCreatedResult_WhenSuccessful()
    {
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

        var result = await controller.CreateCampaign(campaignRequest, CancellationToken.None);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        createdResult.ActionName.Should().Be(nameof(CampaignController.CreateCampaign));
        createdResult.RouteValues.Should().ContainKey("campaignName")
            .WhoseValue.Should().Be(campaignRequest.CampaignName);
    }

    [Fact]
    public async Task GetTracker_ReturnsOkResult_WhenCampaignExists()
    {
        var mediator = new DummyMediator();
        var expectedTracker = new CampaignTrackerDto(
            "Test Campaign",
            "Description",
            [],
            [],
            null,
            [],
            [],
            [],
            [],
            [],
            [],
            []);

        mediator.RegisterHandler<GetCampaignTrackerByName, CampaignTrackerDto>((_, _) => expectedTracker);

        var controller = new CampaignController(mediator);

        var result = await controller.GetTracker("Test Campaign", Guid.NewGuid(), CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedTracker);
    }

    [Fact]
    public async Task RollDice_ReturnsOkResult_WithDiceResult()
    {
        var mediator = new DummyMediator();
        var expectedResult = new DiceRollResult("1d20", 15, [15], 0, "attack");

        mediator.RegisterHandler<RollDiceInCampaign, DiceRollResult>((_, _) => expectedResult);

        var controller = new CampaignController(mediator);
        var request = new RollDiceRequest("1d20", 0, "attack");

        var result = await controller.RollDice("Test Campaign", request, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }
}
