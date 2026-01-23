using DeveloperStore.ORM.MongoDB.Configuration;
using MongoDB.Driver;

namespace DeveloperStore.ORM.MongoDB.Context;

/// <summary>
/// MongoDB context for managing database connections and collections.
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(MongoDbSettings settings)
    {
        _settings = settings;
        
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    /// <summary>
    /// Gets the MongoDB collection for sales events.
    /// </summary>
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

    /// <summary>
    /// Gets the database instance.
    /// </summary>
    public IMongoDatabase Database => _database;
}
