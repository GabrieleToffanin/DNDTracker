namespace DNDTracker.Domain.Primitives;

public abstract class AggregateRoot<TIdentifier>(TIdentifier id) 
    : Entity, IEquatable<AggregateRoot<TIdentifier>> where TIdentifier : notnull
{
    public TIdentifier Id { get; } = id;

    public bool Equals(AggregateRoot<TIdentifier>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<TIdentifier>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AggregateRoot<TIdentifier>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TIdentifier>.Default.GetHashCode(this.Id);
    }

    public static bool operator ==(AggregateRoot<TIdentifier>? left, AggregateRoot<TIdentifier>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(AggregateRoot<TIdentifier>? left, AggregateRoot<TIdentifier>? right)
    {
        return !Equals(left, right);
    }
}