using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Events;

/// <summary>
/// Event raised when a sale is modified.
/// </summary>
public class SaleModifiedEvent
{
    public Sale Sale { get; }

    public DateTime OccurredAt { get; }

    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale;
        OccurredAt = DateTime.UtcNow;
    }
}
