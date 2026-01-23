namespace DeveloperStore.Application.Events.GetAllEvents;

/// <summary>
/// Result containing all retrieved events.
/// </summary>
public class GetAllEventsResult
{
    /// <summary>
    /// List of events retrieved from the store.
    /// </summary>
    public List<EventDto> Events { get; set; } = new();

    /// <summary>
    /// Total number of events returned.
    /// </summary>
    public int Count => Events.Count;
}

/// <summary>
/// Data Transfer Object for an event.
/// </summary>
public class EventDto
{
    /// <summary>
    /// Event identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Type of event (e.g., "SaleCreatedEvent").
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Serialized event data.
    /// </summary>
    public string EventData { get; set; } = string.Empty;

    /// <summary>
    /// When the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// ID of the related sale (if applicable).
    /// </summary>
    public Guid? SaleId { get; set; }

    /// <summary>
    /// Sale number (if applicable).
    /// </summary>
    public string? SaleNumber { get; set; }
}
