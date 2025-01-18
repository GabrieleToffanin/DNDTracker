using DNDTracker.Application.Tests.Behaviors.Dummies;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Domain.Campaigns;
using FluentAssertions;
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
        
        createdCampaign.CampaignName.Should()
            .Be(expectedCampaign.CampaignName);
        
        createdCampaign.CampaignDescription.Should()
            .Be(expectedCampaign.CampaignDescription);
        
        createdCampaign.CampaignImage.Should()
            .Be(expectedCampaign.CampaignImage);
        
        createdCampaign.CreatedDate.Should()
            .Be(expectedCampaign.CreatedDate); 
    }
}