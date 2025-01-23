using DNDTracker.Application.Tests.Behaviors.Dummies;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Exceptions;
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
        
        await _handler.Handle(command, CancellationToken.None);

        var createdCampaign = _campaignRepository.Campaigns.Values.First();
        var expectedCampaign = Campaign.Create(
                command.CampaignName,
                command.CampaignDescription,
                command.CampaignImage,
                createDate,
                true);

        AssertCampaignEquals(createdCampaign, expectedCampaign);
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
    
    private void AssertCampaignEquals(Campaign createdCampaign, Campaign expectedCampaign)
    {
        createdCampaign.CampaignName.Should().Be(expectedCampaign.CampaignName);
        createdCampaign.CampaignDescription.Should().Be(expectedCampaign.CampaignDescription);
        createdCampaign.CampaignImage.Should().Be(expectedCampaign.CampaignImage);
        createdCampaign.IsActive.Should().Be(expectedCampaign.IsActive);
        createdCampaign.CreatedDate.Should().Be(expectedCampaign.CreatedDate);
        createdCampaign.UpdatedDate.Should().Be(expectedCampaign.UpdatedDate);
        createdCampaign.DeletedDate.Should().Be(expectedCampaign.DeletedDate);
        createdCampaign.Heroes.Should().BeEquivalentTo(expectedCampaign.Heroes);
    }
}