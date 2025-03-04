using DNDTracker.Application.Tests.Behaviors.Dummies;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Vocabulary.Exceptions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Xunit;

namespace DNDTracker.Application.Tests;

public sealed class CreateCampaignUseCaseTest
{
    private readonly DummyCampaignRepository _campaignRepository = new();
    private readonly CreateCampaignCommandHandler _handler;

    public CreateCampaignUseCaseTest()
    {
        _handler = new CreateCampaignCommandHandler(_campaignRepository);
    }

    [Fact]
    public async Task GivenValidRequest_WhenHandle_ThenCampaignIsCreated()
    {
        DateTime createDate = DateTime.Now;
        CreateCampaignCommand command = new(
            "Test Campaigns",
            "Test Campaigns Description",
            "testurl.jpg", 
            createDate);
        
        var expectedCampaign = Campaign.Create(
            command.CampaignName,
            command.CampaignDescription,
            command.CampaignImage,
            createDate,
            true);
        
        await _handler.Handle(command, CancellationToken.None);

        _campaignRepository.AssertCampaignEquals(expectedCampaign);
    }
    
    [Fact]
    public async Task GivenNotValidRequest_WhenHandle_ThenCampaignIsNotCreated()
    {
        DateTime createDate = DateTime.Now;
        CreateCampaignCommand command = new(
            "Test Campaigns",
            "Test Campaigns Description",
            "testurl", 
            createDate);
        
        var createAction = async () => await _handler.Handle(command, CancellationToken.None);

        await createAction.Should().ThrowAsync<InvalidCampaignDataException>();
    }
    
    [Fact]
    public async Task GivenEmptyName_WhenHandle_ThenThrowsException()
    {
        // Arrange
        DateTime createDate = DateTime.Now;
        CreateCampaignCommand command = new(
            "",
            "Test Description",
            "testurl.jpg",
            createDate);

        // Act
        var createAction = async () => await _handler.Handle(command, CancellationToken.None);

        await createAction.Should().ThrowAsync<InvalidCampaignDataException>();
    }
}