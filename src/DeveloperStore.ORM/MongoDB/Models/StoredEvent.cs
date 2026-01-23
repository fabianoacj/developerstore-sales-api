using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeveloperStore.ORM.MongoDB.Models;

/// <summary>
/// Represents a domain event stored in MongoDB.
/// </summary>
public class StoredEvent
{
    /// <summary>
    /// MongoDB document identifier.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// Type name of the event (e.g., "SaleCreatedEvent").
    /// </summary>
    [BsonElement("eventType")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Serialized event data as JSON.
    /// </summary>
    [BsonElement("eventData")]
    public string EventData { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the event occurred.
    /// </summary>
    [BsonElement("occurredAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The ID of the aggregate (Sale) this event relates to.
    /// </summary>
    [BsonElement("aggregateId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? AggregateId { get; set; }

    /// <summary>
    /// Optional sale number for easier querying.
    /// </summary>
    [BsonElement("saleNumber")]
    public string? SaleNumber { get; set; }
}
