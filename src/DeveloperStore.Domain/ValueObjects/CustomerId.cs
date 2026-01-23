namespace DeveloperStore.Domain.ValueObjects;

/// <summary>
/// Represents a customer external identity with denormalized data.
/// </summary>
public class CustomerId
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string? Phone { get; private set; }

    public CustomerId(Guid id, string name, string email, string? phone = null)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
    }

    public static CustomerId Create(Guid id, string name, string email, string? phone = null)
        => new(id, name, email, phone);
}
