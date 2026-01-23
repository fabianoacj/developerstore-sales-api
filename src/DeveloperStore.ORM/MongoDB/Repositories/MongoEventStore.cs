using DeveloperStore.Domain.Repositories;
using DeveloperStore.ORM.MongoDB.Configuration;
using DeveloperStore.ORM.MongoDB.Context;
using DeveloperStore.ORM.MongoDB.Models;
using MongoDB.Driver;
using System.Text.Json;

namespace DeveloperStore.ORM.MongoDB.Repositories;

/// <summary>
/// MongoDB implementation of the event store.
/// </summary>
public class MongoEventStore : IEventStore
{
    private readonly IMongoCollection<StoredEvent> _eventsCollection;
    private readonly MongoDbSettings _settings;

    public MongoEventStore(MongoDbContext context, MongoDbSettings settings)
    {
        _settings = settings;
        _eventsCollection = context.GetCollection<StoredEvent>(_settings.EventsCollectionName);
        
        // Create indexes for better query performance
        CreateIndexes();
    }

    /// <summary>
    /// Stores a domain event in MongoDB.
    /// </summary>
    public async Task StoreAsync<TEvent>(
        TEvent @event, 
        Guid? aggregateId = null, 
        string? saleNumber = null, 
        CancellationToken cancellationToken = default) where TEvent : class
    {
        var eventType = typeof(TEvent).Name;
        var eventData = JsonSerializer.Serialize(@event, new JsonSerializerOptions 
        { 
            WriteIndented = false 
        });

        var storedEvent = new StoredEvent
        {
            EventType = eventType,
            EventData = eventData,
            OccurredAt = DateTime.UtcNow,
            AggregateId = aggregateId,
            SaleNumber = saleNumber
        };

        await _eventsCollection.InsertOneAsync(storedEvent, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves all events with pagination.
    /// </summary>
    public async Task<IEnumerable<EventStoreEntry>> GetAllEventsAsync(
        int skip = 0, 
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        var events = await _eventsCollection
            .Find(Builders<StoredEvent>.Filter.Empty)
            .SortByDescending(e => e.OccurredAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);

        return events.Select(MapToEntry);
    }

    /// <summary>
    /// Retrieves all events for a specific sale.
    /// </summary>
    public async Task<IEnumerable<EventStoreEntry>> GetEventsBySaleIdAsync(
        Guid saleId, 
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<StoredEvent>.Filter.Eq(e => e.AggregateId, saleId);
        
        var events = await _eventsCollection
            .Find(filter)
            .SortBy(e => e.OccurredAt)
            .ToListAsync(cancellationToken);

        return events.Select(MapToEntry);
    }

    /// <summary>
    /// Creates indexes for better query performance.
    /// </summary>
    private void CreateIndexes()
    {
        var indexKeys = Builders<StoredEvent>.IndexKeys
            .Descending(e => e.OccurredAt);
        var indexModel = new CreateIndexModel<StoredEvent>(indexKeys);
        _eventsCollection.Indexes.CreateOne(indexModel);

        var aggregateIndexKeys = Builders<StoredEvent>.IndexKeys
            .Ascending(e => e.AggregateId);
        var aggregateIndexModel = new CreateIndexModel<StoredEvent>(aggregateIndexKeys);
        _eventsCollection.Indexes.CreateOne(aggregateIndexModel);
    }

    /// <summary>
    /// Maps StoredEvent to EventStoreEntry.
    /// </summary>
    private static EventStoreEntry MapToEntry(StoredEvent storedEvent)
    {
        return new EventStoreEntry
        {
            Id = storedEvent.Id,
            EventType = storedEvent.EventType,
            EventData = storedEvent.EventData,
            OccurredAt = storedEvent.OccurredAt,
            AggregateId = storedEvent.AggregateId,
            SaleNumber = storedEvent.SaleNumber
        };
    }
}
