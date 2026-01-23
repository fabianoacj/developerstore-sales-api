namespace DeveloperStore.Domain.Repositories;

/// <summary>
/// Repository interface for storing and retrieving domain events.
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Stores a domain event in the event store.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to store.</typeparam>
    /// <param name="event">The event instance.</param>
    /// <param name="aggregateId">Optional aggregate (Sale) ID.</param>
    /// <param name="saleNumber">Optional sale number for easier querying.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StoreAsync<TEvent>(
        TEvent @event, 
        Guid? aggregateId = null, 
        string? saleNumber = null,
        CancellationToken cancellationToken = default) where TEvent : class;

    /// <summary>
    /// Retrieves all events from the event store.
    /// </summary>
    /// <param name="skip">Number of events to skip (for pagination).</param>
    /// <param name="limit">Maximum number of events to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IEnumerable<EventStoreEntry>> GetAllEventsAsync(
        int skip = 0, 
        int limit = 100, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all events related to a specific sale.
    /// </summary>
    /// <param name="saleId">The sale ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IEnumerable<EventStoreEntry>> GetEventsBySaleIdAsync(
        Guid saleId, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an event entry retrieved from the store.
/// </summary>
public class EventStoreEntry
{
    public string Id { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public Guid? AggregateId { get; set; }
    public string? SaleNumber { get; set; }
}
