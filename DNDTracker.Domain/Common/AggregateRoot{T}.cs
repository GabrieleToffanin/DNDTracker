namespace DNDTracker.Domain.Common;

public abstract class AggregateRoot<TIdentifier> : Entity, IEquatable<AggregateRoot<TIdentifier>>
{
    protected AggregateRoot(TIdentifier id)
    {
        this.Id = id;
    }
    
    public TIdentifier Id { get; protected set; }

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
        return EqualityComparer<TIdentifier>.Default.GetHashCode(Id);
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