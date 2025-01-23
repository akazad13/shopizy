using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Shopizy.Application.Common.Caching;

/// <summary>
/// Helper class for interacting with Redis cache.
/// </summary>
public class RedisCacheHelper : ICacheHelper
{
    private readonly RedisSettings _redisSettings;
    private readonly Lazy<ConnectionMultiplexer> _redisDbConnectionLazy;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheHelper"/> class.
    /// </summary>
    /// <param name="redisSettingsOptions">The Redis settings options.</param>
    public RedisCacheHelper(IOptions<RedisSettings> redisSettingsOptions)
    {
        _redisSettings = redisSettingsOptions.Value;
        _redisDbConnectionLazy = new Lazy<ConnectionMultiplexer>(
            () =>
                ConnectionMultiplexer.Connect(
                    new ConfigurationOptions
                    {
                        EndPoints = { { _redisSettings.Endpoint, 16199 } },
                        User = _redisSettings.Username,
                        Password = _redisSettings.Password,
                    }
                ),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    /// <summary>
    /// Gets the cached value for the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value, or default if the key does not exist.</returns>
    public async Task<T> GetAsync<T>(string key)
    {
        var db = _redisDbConnectionLazy.Value.GetDatabase();
        var data = await db.StringGetAsync(key);

        return data.HasValue ? JsonSerializer.Deserialize<T>(data) : default;
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
        var db = _redisDbConnectionLazy.Value.GetDatabase();
        await db.StringSetAsync(key, JsonSerializer.Serialize(value), expiration);
    }

    /// <summary>
    /// Removes the cached value for the specified key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    public async Task RemoveAsync(string key)
    {
        var db = _redisDbConnectionLazy.Value.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
