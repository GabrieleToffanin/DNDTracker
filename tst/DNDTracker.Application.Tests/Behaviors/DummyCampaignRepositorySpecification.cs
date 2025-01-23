using DNDTracker.Application.Tests.Behaviors.Dummies;
using DNDTracker.Domain.Tests.Behaviors;

namespace DNDTracker.Application.Tests.Behaviors;

public class DummyCampaignRepositorySpecification : CampaignRepositorySpecification
{
    public DummyCampaignRepositorySpecification()
    {
        _campaignRepository = new DummyCampaignRepository();
    }
}