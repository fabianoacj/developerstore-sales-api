using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Events;

/// <summary>
/// Event raised when a sale item is cancelled.
/// </summary>
public class ItemCancelledEvent
{
    public SaleItem Item { get; }

    public Guid SaleId { get; }

    public DateTime OccurredAt { get; }

    public ItemCancelledEvent(SaleItem item, Guid saleId)
    {
        Item = item;
        SaleId = saleId;
        OccurredAt = DateTime.UtcNow;
    }
}
