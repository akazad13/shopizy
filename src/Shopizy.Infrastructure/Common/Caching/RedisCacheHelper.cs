using System.Text.Json;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Caching;
using StackExchange.Redis;

#pragma warning disable CA1848 // Use the LoggerMessage delegates

namespace Shopizy.Infrastructure.Common.Caching;

/// <summary>
/// Helper class for interacting with Redis cache.
/// </summary>
public class RedisCacheHelper(
    IConnectionMultiplexer connectionMultiplexer,
    ILogger<RedisCacheHelper> logger
) : ICacheHelper
{
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
    private readonly ILogger<RedisCacheHelper> _logger = logger;

    /// <summary>
    /// Gets the cached value for the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value, or default if the key does not exist.</returns>
    public async Task<T> GetAsync<T>(string key)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var data = await db.StringGetAsync(key);
            
            if (!data.HasValue)
            {
                return default!;
            }

            return JsonSerializer.Deserialize<T>(data.ToString())!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving key {Key} from Redis", key);
            return default;
        }
    }

    /// <summary>
    /// Sets the specified value in the cache with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">The expiration time for the cached value. If null, the value does not expire.</param>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value);
            if (expiration.HasValue)
            {
                await db.StringSetAsync(key, serializedValue, expiry: expiration.Value, when: When.Always, flags: CommandFlags.None);
            }
            else
            {
                await db.StringSetAsync(key, serializedValue, expiry: null, when: When.Always, flags: CommandFlags.None);
            }
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error setting key {Key} in Redis", key);
        }
    }

    /// <summary>
    /// Removes the cached value for the specified key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    public async Task RemoveAsync(string key)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error removing key {Key} from Redis", key);
        }
    }
}
