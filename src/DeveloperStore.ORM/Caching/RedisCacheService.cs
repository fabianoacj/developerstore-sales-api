using DeveloperStore.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace DeveloperStore.ORM.Caching;

/// <summary>
/// Redis implementation of the cache service.
/// Uses StackExchange.Redis for high-performance caching.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly RedisConfiguration _configuration;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of RedisCacheService.
    /// </summary>
    /// <param name="redis">Redis connection multiplexer.</param>
    /// <param name="configuration">Redis configuration.</param>
    /// <param name="logger">Logger instance.</param>
    public RedisCacheService(
        IConnectionMultiplexer redis,
        IOptions<RedisConfiguration> configuration,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _configuration = configuration.Value;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (!_configuration.Enabled)
        {
            _logger.LogDebug("Cache is disabled. Skipping cache retrieval for key: {Key}", key);
            return null;
        }

        try
        {
            var cacheKey = GetFullKey(key);
            var cachedValue = await _database.StringGetAsync(cacheKey);

            if (!cachedValue.HasValue)
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            _logger.LogDebug("Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedValue!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value from cache for key: {Key}", key);
            return null; // Fail gracefully - return null and let the application fetch from source
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        if (!_configuration.Enabled)
        {
            _logger.LogDebug("Cache is disabled. Skipping cache set for key: {Key}", key);
            return;
        }

        try
        {
            var cacheKey = GetFullKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);

            var effectiveExpiration = expiration ?? 
                (_configuration.DefaultExpirationMinutes.HasValue && _configuration.DefaultExpirationMinutes > 0
                    ? TimeSpan.FromMinutes(_configuration.DefaultExpirationMinutes.Value)
                    : (TimeSpan?)null);

            if (effectiveExpiration.HasValue)
            {
                await _database.StringSetAsync(cacheKey, serializedValue, effectiveExpiration.Value);
                _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, effectiveExpiration);
            }
            else
            {
                await _database.StringSetAsync(cacheKey, serializedValue);
                _logger.LogDebug("Cached value for key: {Key} with no expiration", key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
            // Fail gracefully - don't throw, just log the error
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_configuration.Enabled)
        {
            _logger.LogDebug("Cache is disabled. Skipping cache removal for key: {Key}", key);
            return;
        }

        try
        {
            var cacheKey = GetFullKey(key);
            var removed = await _database.KeyDeleteAsync(cacheKey);
            
            if (removed)
            {
                _logger.LogDebug("Removed cache entry for key: {Key}", key);
            }
            else
            {
                _logger.LogDebug("Cache key not found for removal: {Key}", key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing value from cache for key: {Key}", key);
            // Fail gracefully
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_configuration.Enabled)
        {
            return false;
        }

        try
        {
            var cacheKey = GetFullKey(key);
            return await _database.KeyExistsAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if key exists in cache: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Gets the full cache key with instance name prefix.
    /// </summary>
    /// <param name="key">The base cache key.</param>
    /// <returns>The full cache key with prefix.</returns>
    private string GetFullKey(string key)
    {
        return $"{_configuration.InstanceName}{key}";
    }
}
