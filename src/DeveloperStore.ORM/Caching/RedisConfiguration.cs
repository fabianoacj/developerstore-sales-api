namespace DeveloperStore.ORM.Caching;

/// <summary>
/// Configuration settings for Redis cache.
/// </summary>
public class RedisConfiguration
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// Redis connection string.
    /// Format: {host}:{port},password={password}
    /// Example: localhost:6379,password=mypassword
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Instance name prefix for cache keys.
    /// Useful when multiple applications share the same Redis instance.
    /// </summary>
    public string InstanceName { get; set; } = "DeveloperStore:";

    /// <summary>
    /// Default expiration time for cached items in minutes.
    /// Set to 0 or null for no expiration.
    /// </summary>
    public int? DefaultExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Enable/disable caching globally.
    /// Useful for development or troubleshooting.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
