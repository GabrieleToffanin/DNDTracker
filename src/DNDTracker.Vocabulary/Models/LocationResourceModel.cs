using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class LocationResourceModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? MapUrl { get; set; }
    public CampaignModel Campaign { get; set; } = null!;

    public static LocationResourceModel From(LocationResource location) => new()
    {
        Id = location.Id,
        Name = location.Name,
        Description = location.Description,
        MapUrl = location.MapUrl
    };

    public LocationResource ToValueObject() => new(Id, Name, Description, MapUrl);

    public void Apply(LocationResourceModel source)
    {
        Name = source.Name;
        Description = source.Description;
        MapUrl = source.MapUrl;
    }
}
