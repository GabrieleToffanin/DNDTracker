namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record DeathSaves(int Successes, int Failures)
{
    public static readonly DeathSaves None = new(0, 0);

    public bool IsStabilized => Successes >= 3;
    public bool IsDead => Failures >= 3;
}
