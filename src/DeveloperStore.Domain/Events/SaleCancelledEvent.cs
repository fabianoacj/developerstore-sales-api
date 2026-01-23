using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Events;

/// <summary>
/// Event raised when a sale is cancelled.
/// </summary>
public class SaleCancelledEvent
{
    public Sale Sale { get; }

    public DateTime OccurredAt { get; }

    public SaleCancelledEvent(Sale sale)
    {
        Sale = sale;
        OccurredAt = DateTime.UtcNow;
    }
}
