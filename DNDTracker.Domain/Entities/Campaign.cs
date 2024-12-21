using DNDTracker.Domain.Common;
using DNDTracker.Domain.DomainEvents;

namespace DNDTracker.Domain.Entities;

public sealed class Campaign : AggregateRoot<CampaignId>
{
    private Campaign(
        CampaignId id,
        string campaignName,
        string campaignDescription,
        string campaignImage,
        bool isActive,
        DateTime createdDate,
        DateTime updatedDate,
        DateTime? deletedDate) : base(id)
    {
        CampaignName = campaignName;
        CampaignDescription = campaignDescription;
        CampaignImage = campaignImage;
        IsActive = isActive;
        CreatedDate = createdDate;
        UpdatedDate = updatedDate;
        DeletedDate = deletedDate;
    }

    public string CampaignName { get; private set; }
    public string CampaignDescription { get; private set; }
    public string CampaignImage { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }
    public DateTime? DeletedDate { get; private set; }
    
    public List<Hero> Heroes { get; private set; } = [];

    /// <summary>
    /// Creates a new campaign with the provided details.
    /// </summary>
    /// <param name="campaignName">The name of the campaign.</param>
    /// <param name="campaignDescription">The description of the campaign.</param>
    /// <param name="campaignImage">The representative image for the campaign.</param>
    /// <param name="isActive">A boolean indicating whether the campaign is active.</param>
    /// <returns>A new instance of the <see cref="Campaign"/> class.</returns>
    public static Campaign Create(
        string campaignName,
        string campaignDescription,
        string campaignImage,
        bool isActive)
    {
        var id = CampaignId.Create();
        var currentDate = DateTime.UtcNow;

        return new Campaign(
            id,
            campaignName,
            campaignDescription,
            campaignImage,
            isActive,
            currentDate,
            currentDate,
            null
        );
    }

    /// <summary>
    /// Adds a hero to the campaign and triggers the corresponding domain event.
    /// </summary>
    /// <param name="hero">The hero to add to the campaign.</param>
    public void AddHero(Hero hero)
    {
        ArgumentNullException.ThrowIfNull(hero);
        
        // Add the hero to the domain event collection.
        HeroAddedDomainEvent heroAddedEvent = new(Guid.NewGuid(), DateTime.UtcNow);
        
        this.AddDomainEvent(heroAddedEvent);
    }
}