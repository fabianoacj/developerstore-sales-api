namespace DeveloperStore.ORM.MongoDB.Configuration;

/// <summary>
/// Configuration settings for MongoDB connection.
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// MongoDB connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database name for storing events.
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>
    /// Collection name for sales events.
    /// </summary>
    public string EventsCollectionName { get; set; } = "sales_events";
}
