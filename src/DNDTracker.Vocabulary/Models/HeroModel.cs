using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.Models;

public class HeroModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public HeroClass Class { get; init; }
    public Race Race { get; init; }
    public Alignment Alignment { get; init; }
    public int Level { get; init; }
    public int Experience { get; init; }
    public int HitPoints { get; init; }
    public DiceType HitDice { get; init; }
    public HashSet<SpellModel> Spells { get; } = [];
    public CampaignModel Campaign { get; init; }
}