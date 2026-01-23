using DeveloperStore.Application.Events.GetAllEvents;

namespace DeveloperStore.Application.Events.GetSaleEvents;

/// <summary>
/// Result containing events for a specific sale.
/// </summary>
public class GetSaleEventsResult
{
    /// <summary>
    /// The sale ID these events belong to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Timeline of events for this sale.
    /// </summary>
    public List<EventDto> Events { get; set; } = new();

    /// <summary>
    /// Total number of events for this sale.
    /// </summary>
    public int Count => Events.Count;
}
