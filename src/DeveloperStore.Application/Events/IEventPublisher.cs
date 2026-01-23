namespace DeveloperStore.Application.Events;

/// <summary>
/// Interface for publishing domain events.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
}
