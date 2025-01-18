namespace DNDTracker.Domain.Heroes;

public sealed record HeroId
{
    public Guid Id { get; init; }

    public HeroId()
    {
        this.Id = Guid.NewGuid();
    }
    
    public HeroId(Guid id)
    {
        this.Id = id;
    }
    
    public static HeroId Create()
        => new HeroId();
    
    public static HeroId Create(Guid id)
        => new HeroId(id);
}