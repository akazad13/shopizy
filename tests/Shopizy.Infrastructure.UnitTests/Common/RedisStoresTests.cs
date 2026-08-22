using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Idempotency;
using Shopizy.Infrastructure.Security.RefreshTokens;
using Shouldly;
using StackExchange.Redis;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Common;

public class RedisStoresTests
{
    [Fact]
    public async Task RedisIdempotencyStore_TryGetAndStore_ShouldHandleDataAndExceptions()
    {
        var mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDb = new Mock<IDatabase>();
        var mockLogger = new Mock<ILogger<RedisIdempotencyStore>>();

        mockRedis
            .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        var record = new IdempotencyRecord("hash-123", 200, "application/json", [1, 2, 3]);
        var serialized = JsonSerializer.Serialize(record);

        mockDb
            .Setup(d => d.StringGetAsync("idempotency:key-1", It.IsAny<CommandFlags>()))
            .ReturnsAsync(serialized);

        var store = new RedisIdempotencyStore(mockRedis.Object, mockLogger.Object);

        var result = await store.TryGetAsync("key-1");
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        await store.StoreAsync("key-1", record, TimeSpan.FromMinutes(5));

        // Test exception branch
        mockDb
            .Setup(d => d.StringGetAsync("idempotency:err-key", It.IsAny<CommandFlags>()))
            .Throws(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Fail"));

        var nullResult = await store.TryGetAsync("err-key");
        nullResult.ShouldBeNull();
    }

    [Fact]
    public async Task RedisRefreshTokenStore_StoreConsumeAndRevoke_ShouldWorkCorrectly()
    {
        var mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDb = new Mock<IDatabase>();
        var mockBatch = new Mock<IBatch>();
        var mockLogger = new Mock<ILogger<RedisRefreshTokenStore>>();

        mockRedis
            .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);
        mockDb.Setup(d => d.CreateBatch(It.IsAny<object>())).Returns(mockBatch.Object);

        var store = new RedisRefreshTokenStore(mockRedis.Object, mockLogger.Object);
        var userId = UserId.CreateUnique();

        mockBatch
            .Setup(b =>
                b.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan?>(),
                    false,
                    When.Always,
                    CommandFlags.None
                )
            )
            .Returns(Task.FromResult(true));
        mockBatch
            .Setup(b =>
                b.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None)
            )
            .Returns(Task.FromResult(true));
        mockBatch
            .Setup(b =>
                b.KeyExpireAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<TimeSpan?>(),
                    ExpireWhen.Always,
                    CommandFlags.None
                )
            )
            .Returns(Task.FromResult(true));

        await store.StoreAsync("token-123", userId, TimeSpan.FromDays(7));

        mockDb
            .Setup(d => d.StringGetDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None))
            .ReturnsAsync(userId.Value.ToString());

        var consumed = await store.ConsumeAsync("token-123");
        consumed.ShouldNotBeNull();
        consumed.Value.ShouldBe(userId.Value);

        await store.RevokeAsync("token-123");
        mockDb.Verify(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None), Times.Once);

        mockDb
            .Setup(d => d.SetMembersAsync(It.IsAny<RedisKey>(), CommandFlags.None))
            .ReturnsAsync(new RedisValue[] { "hash-1" });

        await store.RevokeAllForUserAsync(userId);
    }
}
