using DeveloperStore.Domain.Events;
using DeveloperStore.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace DeveloperStore.Application.Events;

/// <summary>
/// Event publisher that stores events in MongoDB event store.
/// Implements decorator pattern - can wrap another publisher for chaining.
/// </summary>
public class MongoDbEventPublisher : IEventPublisher
{
    private readonly IEventStore _eventStore;
    private readonly IEventPublisher? _innerPublisher;
    private readonly ILogger<MongoDbEventPublisher> _logger;

    public MongoDbEventPublisher(
        IEventStore eventStore,
        ILogger<MongoDbEventPublisher> logger,
        IEventPublisher? innerPublisher = null)
    {
        _eventStore = eventStore;
        _logger = logger;
        _innerPublisher = innerPublisher;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        try
        {
            // Extract Sale information based on event type
            var (aggregateId, saleNumber) = ExtractSaleInfo(@event);

            // Store event in MongoDB
            await _eventStore.StoreAsync(@event, aggregateId, saleNumber, cancellationToken);

            _logger.LogInformation(
                "Event {EventType} stored in MongoDB event store for Sale {SaleNumber}",
                typeof(TEvent).Name,
                saleNumber ?? "N/A");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to store event {EventType} in MongoDB event store", 
                typeof(TEvent).Name);
            // Don't rethrow - event publishing should be resilient
        }

        // Chain to inner publisher if configured (e.g., LoggerEventPublisher)
        if (_innerPublisher != null)
        {
            await _innerPublisher.PublishAsync(@event, cancellationToken);
        }
    }

    /// <summary>
    /// Extracts Sale ID and Sale Number from domain events.
    /// </summary>
    private static (Guid? AggregateId, string? SaleNumber) ExtractSaleInfo<TEvent>(TEvent @event) where TEvent : class
    {
        return @event switch
        {
            SaleCreatedEvent created => (created.Sale.Id, created.Sale.SaleNumber),
            SaleModifiedEvent modified => (modified.Sale.Id, modified.Sale.SaleNumber),
            SaleCancelledEvent cancelled => (cancelled.Sale.Id, cancelled.Sale.SaleNumber),
            SaleItemCancelledEvent itemCancelled => (itemCancelled.Sale.Id, itemCancelled.Sale.SaleNumber),
            _ => (null, null)
        };
    }
}
