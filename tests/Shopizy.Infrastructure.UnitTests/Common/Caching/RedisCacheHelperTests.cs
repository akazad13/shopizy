using Xunit;
using Moq;
using Shouldly;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Shopizy.Infrastructure.Common.Caching;
using System.Text.Json;

namespace Shopizy.Infrastructure.UnitTests.Common.Caching;

public class RedisCacheHelperTests
{
    private readonly Mock<IConnectionMultiplexer> _mockMultiplexer;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly Mock<ILogger<RedisCacheHelper>> _mockLogger;
    private readonly RedisCacheHelper _cacheHelper;

    public RedisCacheHelperTests()
    {
        _mockMultiplexer = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        _mockLogger = new Mock<ILogger<RedisCacheHelper>>();

        _mockMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _cacheHelper = new RedisCacheHelper(_mockMultiplexer.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAsync_WhenKeyExists_ShouldReturnDeserializedValue()
    {
        // Arrange
        var key = "test-key";
        var value = new TestDto { Name = "Test" };
        var serializedValue = JsonSerializer.Serialize(value);
        _mockDatabase.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(serializedValue);

        // Act
        var result = await _cacheHelper.GetAsync<TestDto>(key);

        // Assert
        result.Success.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("Test");
    }

    [Fact]
    public async Task GetAsync_WhenKeyDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        _mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await _cacheHelper.GetAsync<TestDto>("missing");

        // Assert
        result.Success.ShouldBeFalse();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldCallStringSetAsync()
    {
        // Arrange
        var key = "set-key";
        var value = new TestDto { Name = "Set" };

        _mockDatabase.Setup(db => db.StringSetAsync(
            It.IsAny<RedisKey>(),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<bool>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _mockDatabase.Setup(db => db.StringSetAsync(
            It.IsAny<RedisKey>(),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _cacheHelper.SetAsync(key, value);

        // Assert
        // Explicitly cast to IDatabaseAsync to ensure we're verifying the interface member correctly
        var databaseAsync = _mockDatabase.As<IDatabaseAsync>();
        
        try 
        {
            databaseAsync.Verify(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        } 
        catch 
        {
            databaseAsync.Verify(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }
    }

    private class TestDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
