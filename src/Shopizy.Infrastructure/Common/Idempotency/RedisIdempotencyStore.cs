using System.Text.Json;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Services;
using StackExchange.Redis;

namespace Shopizy.Infrastructure.Common.Idempotency;

public sealed class RedisIdempotencyStore(
    IConnectionMultiplexer connection,
    ILogger<RedisIdempotencyStore> logger) : IIdempotencyStore
{
    private const string KeyPrefix = "idempotency:";

    public async Task<IdempotencyRecord?> TryGetAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = connection.GetDatabase();
            var data = await db.StringGetAsync(KeyPrefix + key);
            return data.HasValue
                ? JsonSerializer.Deserialize<IdempotencyRecord>(data.ToString())
                : null;
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable while reading idempotency key {Key}", key);
            return null;
        }
    }

    public async Task StoreAsync(string key, IdempotencyRecord record, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = connection.GetDatabase();
            var serialized = JsonSerializer.Serialize(record);
            await db.StringSetAsync(KeyPrefix + key, serialized, ttl, when: When.Always);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable while storing idempotency key {Key}", key);
        }
    }
}
