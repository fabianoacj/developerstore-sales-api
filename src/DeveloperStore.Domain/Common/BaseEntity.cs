namespace DeveloperStore.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides common properties and functionality for all entities in the domain.
/// </summary>
public abstract class BaseEntity : IComparable<BaseEntity>
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CompareTo(BaseEntity? other)
    {
        if (other == null)
        {
            return 1;
        }

        return other!.Id.CompareTo(Id);
    }
}
