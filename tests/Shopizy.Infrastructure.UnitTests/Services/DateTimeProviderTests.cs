using Shopizy.Infrastructure.Services;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Services;


public class DateTimeProviderTests
{
    [Fact]
    public void UtcNow_ReturnsRecentTime()
    {
        // Arrange
        var provider = new DateTimeProvider();

        // Act
        var utcNow = provider.UtcNow;

        // Assert
        utcNow.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
    }

    /// <summary>
    /// Tests that UtcNow returns a DateTime with DateTimeKind.Utc.
    /// This ensures the returned time is explicitly marked as UTC, not local or unspecified.
    /// </summary>
    [Fact]
    public void UtcNow_ReturnsUtcKind()
    {
        // Arrange
        var provider = new DateTimeProvider();

        // Act
        var utcNow = provider.UtcNow;

        // Assert
        utcNow.Kind.ShouldBe(DateTimeKind.Utc);
    }

    /// <summary>
    /// Tests that consecutive calls to UtcNow return times that are either equal or advancing.
    /// This verifies the property behaves consistently across multiple invocations.
    /// </summary>
    [Fact]
    public void UtcNow_ConsecutiveCalls_ReturnConsistentOrAdvancingTime()
    {
        // Arrange
        var provider = new DateTimeProvider();

        // Act
        var firstCall = provider.UtcNow;
        var secondCall = provider.UtcNow;
        var thirdCall = provider.UtcNow;

        // Assert
        secondCall.ShouldBeGreaterThanOrEqualTo(firstCall);
        thirdCall.ShouldBeGreaterThanOrEqualTo(secondCall);
    }
}