namespace DNDTracker.Vocabulary.Models;

public class HeroFeatModel
{
    public Guid Id { get; set; }
    public Guid HeroId { get; set; }
    public string FeatName { get; set; } = string.Empty;
    public HeroModel Hero { get; set; } = null!;

    public static HeroFeatModel From(Guid heroId, string featName) => new()
    {
        Id = Guid.NewGuid(),
        HeroId = heroId,
        FeatName = featName
    };
}
