using System.Runtime.CompilerServices;
using DNDTracker.Domain.Campaigns.DomainEvents;
using DNDTracker.Domain.Heroes;
using DNDTracker.SharedKernel.Primitives;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Domain.Campaigns;

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
        DateTime? deletedDate,
        List<Hero> heroes) : base(id)
    {
        CampaignName = campaignName;
        CampaignDescription = campaignDescription;
        CampaignImage = campaignImage;
        IsActive = isActive;
        CreatedDate = DateTime.SpecifyKind(createdDate, DateTimeKind.Utc);
        UpdatedDate = DateTime.SpecifyKind(updatedDate, DateTimeKind.Utc);
        DeletedDate = deletedDate is not null ? DateTime.SpecifyKind(deletedDate.Value, DateTimeKind.Utc) : null;
        Heroes = heroes;
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
    /// Creates a new campaign entity with the specified details.
    /// </summary>
    /// <param name="id">CampaignId</param>
    /// <param name="campaignName">The name of the campaign.</param>
    /// <param name="campaignDescription">A description of the campaign.</param>
    /// <param name="campaignImage">The URL or path to the campaign's image.</param>
    /// <param name="createdDate">The date the campaign was created.</param>
    /// <param name="isActive">Indicates whether the campaign is currently active.</param>
    /// <param name="heroes">Heroes to be added to the Campaign</param>
    /// <returns>A new instance of the <see cref="Campaign"/> class initialized with the provided details.</returns>
    public static Campaign Create(
        Guid? id, 
        string campaignName,
        string campaignDescription,
        string campaignImage,
        DateTime createdDate,
        bool isActive,
        List<Hero> heroes)
    {
        ThrowIfInvalidName(campaignName);
        ThrowIfInvalidDescription(campaignDescription);
        ThrowIfInvalidImage(campaignImage);
            
        var currentId = id is not null ? CampaignId.Create(id.Value) : CampaignId.Create();

        return new Campaign(
            currentId,
            campaignName,
            campaignDescription,
            campaignImage,
            isActive,
            createdDate,
            createdDate,
            null,
            heroes);
    }

    /// <summary>
    /// Adds a hero to the campaign and triggers the corresponding domain event.
    /// </summary>
    /// <param name="hero">The hero to add to the campaign.</param>
    public void AddHero(params Hero[] hero)
    {
        ArgumentNullException.ThrowIfNull(hero);
        
        this.Heroes.AddRange(hero);
        
        // Add the hero to the domain event collection.
        HeroAddedDomainEvent heroAddedEvent = new(Guid.NewGuid(), DateTime.UtcNow);
        
        this.AddDomainEvent(heroAddedEvent);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidName(string campaignName)
    {
        if (!HasValidCampaignName(campaignName))
            throw new InvalidCampaignDataException($"Invalid campaign name. {campaignName}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidDescription(string campaignDescription)
    {
        if (!HasValidCampaignDescription(campaignDescription))
            throw new InvalidCampaignDataException($"Invalid campaign description. {campaignDescription}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidImage(string campaignImage)
    {
        if (!HasValidCampaignImage(campaignImage))
            throw new InvalidCampaignDataException($"Invalid campaign image. {campaignImage}");
    }

    private static bool HasValidCampaignName(string campaignName)
    {
        return !string.IsNullOrWhiteSpace(campaignName);
    }
    
    private static bool HasValidCampaignDescription(string campaignDescription)
    {
        return !string.IsNullOrWhiteSpace(campaignDescription);
    }

    private static bool HasValidCampaignImage(string campaignImage)
    {
        string extension = Path.GetExtension(campaignImage).ToLower();
        return extension == ".jpg" || extension == ".jpeg" || extension == ".png";
    }

    public static Campaign Create(
        string requestCampaignName,
        string requestCampaignDescription,
        string requestCampaignImage,
        DateTime requestCreatedDate,
        bool requestIsActive)
    {
        ThrowIfInvalidName(requestCampaignName);
        ThrowIfInvalidDescription(requestCampaignDescription);
        ThrowIfInvalidImage(requestCampaignImage);
            
        var currentId = CampaignId.Create();

        return new Campaign(
            currentId,
            requestCampaignName,
            requestCampaignDescription,
            requestCampaignImage,
            requestIsActive,
            requestCreatedDate,
            requestCreatedDate,
            null,
            []);
    }
}