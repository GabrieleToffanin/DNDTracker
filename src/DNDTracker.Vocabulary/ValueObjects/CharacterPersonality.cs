namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record CharacterPersonality(
    string PersonalityTraits,
    string Ideals,
    string Bonds,
    string Flaws)
{
    public static readonly CharacterPersonality Empty = new(string.Empty, string.Empty, string.Empty, string.Empty);
}
