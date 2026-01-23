namespace DeveloperStore.Domain.ValueObjects;

/// <summary>
/// Represents a branch external identity with denormalized data.
/// </summary>
public class BranchId
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public BranchId(Guid id, string name)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public static BranchId Create(Guid id, string name) => new(id, name);
}
