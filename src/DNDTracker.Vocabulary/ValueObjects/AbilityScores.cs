namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record AbilityScores(
    int Strength,
    int Dexterity,
    int Constitution,
    int Intelligence,
    int Wisdom,
    int Charisma)
{
    public static readonly AbilityScores Default = new(10, 10, 10, 10, 10, 10);
}
