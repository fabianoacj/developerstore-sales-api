using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Events;

/// <summary>
/// Event raised when a sale item is cancelled.
/// </summary>
public class SaleItemCancelledEvent
{
    public Sale Sale { get; }
    public SaleItem Item { get; }
    public DateTime OccurredAt { get; }

    public SaleItemCancelledEvent(Sale sale, SaleItem item)
    {
        Sale = sale;
        Item = item;
        OccurredAt = DateTime.UtcNow;
    }
}
