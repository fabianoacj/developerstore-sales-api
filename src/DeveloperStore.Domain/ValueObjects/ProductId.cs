namespace DeveloperStore.Domain.ValueObjects;

/// <summary>
/// Represents a product external identity with denormalized data.
/// </summary>
public class ProductId
{
    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string Category { get; private set; }

    public string? Description { get; private set; }

    public ProductId(Guid id, string title, string category, string? description = null)
    {
        Id = id;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Description = description;
    }

    public static ProductId Create(Guid id, string title, string category, string? description = null)
        => new(id, title, category, description);
}
