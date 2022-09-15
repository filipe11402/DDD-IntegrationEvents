namespace Hospital.API.Domain.Abstract;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }

    public Entity()
    {
    }

    public Entity(Guid id)
    {
        Id = id;
    }

    public bool Equals(Entity? other)
    {
        if (other is null) { return false; }

        return ReferenceEquals(this, other);
    }
}
