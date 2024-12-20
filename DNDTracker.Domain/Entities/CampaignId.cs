namespace DNDTracker.Domain.Entities;

public sealed record CampaignId
{
    public Guid Id { get; init; }

    private CampaignId()
    {
        this.Id = Guid.NewGuid();
    }
    
    private CampaignId(Guid id)
    {
        this.Id = id;
    }

    public static CampaignId Create()
        => new CampaignId();

    public static CampaignId Create(Guid id)
        => new CampaignId(id);
}