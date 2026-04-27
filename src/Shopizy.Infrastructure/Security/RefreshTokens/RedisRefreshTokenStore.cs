using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Domain.Users.ValueObjects;
using StackExchange.Redis;

namespace Shopizy.Infrastructure.Security.RefreshTokens;

public sealed class RedisRefreshTokenStore(
    IConnectionMultiplexer connection,
    ILogger<RedisRefreshTokenStore> logger) : IRefreshTokenStore
{
    private const string KeyPrefix = "refresh:";
    private const string UserIndexPrefix = "refresh-user:";

    public async Task StoreAsync(string token, UserId userId, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var hash = Hash(token);
        try
        {
            var db = connection.GetDatabase();
            var batch = db.CreateBatch();
            var setTask = batch.StringSetAsync(KeyPrefix + hash, userId.Value.ToString(), ttl, when: When.Always);
            var indexTask = batch.SetAddAsync(UserIndexPrefix + userId.Value, hash);
            var indexExpireTask = batch.KeyExpireAsync(UserIndexPrefix + userId.Value, ttl);
            batch.Execute();
            await setTask;
            await indexTask;
            await indexExpireTask;
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable while storing refresh token for {UserId}", userId.Value);
        }
    }

    public async Task<UserId?> ConsumeAsync(string token, CancellationToken cancellationToken = default)
    {
        var hash = Hash(token);
        try
        {
            var db = connection.GetDatabase();
            var value = await db.StringGetDeleteAsync(KeyPrefix + hash);
            if (!value.HasValue || !Guid.TryParse(value.ToString(), out var userGuid))
            {
                return null;
            }
            await db.SetRemoveAsync(UserIndexPrefix + userGuid, hash);
            return UserId.Create(userGuid);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable while consuming refresh token");
            return null;
        }
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var hash = Hash(token);
        try
        {
            var db = connection.GetDatabase();
            await db.KeyDeleteAsync(KeyPrefix + hash);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable while revoking refresh token");
        }
    }

    public async Task RevokeAllForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        try
        {
            var db = connection.GetDatabase();
            var hashes = await db.SetMembersAsync(UserIndexPrefix + userId.Value);
            foreach (var entry in hashes)
            {
                if (entry.HasValue)
                {
                    await db.KeyDeleteAsync(KeyPrefix + entry.ToString());
                }
            }
            await db.KeyDeleteAsync(UserIndexPrefix + userId.Value);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable while revoking refresh tokens for {UserId}", userId.Value);
        }
    }

    private static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
