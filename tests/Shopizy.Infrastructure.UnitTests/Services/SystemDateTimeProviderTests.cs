using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Infrastructure.Services;
using Shouldly;
using System;
using Xunit;


namespace Shopizy.Infrastructure.Services.UnitTests;

public class SystemDateTimeProviderTests
{
    /// <summary>
    /// Tests that UtcNow returns a DateTime value close to the current UTC time.
    /// Input: None (property access).
    /// Expected: Returned value should fall between DateTime.UtcNow captured before and after the call.
    /// </summary>
    [Fact]
    public void UtcNow_WhenAccessed_ReturnsCurrentUtcTime()
    {
        // Arrange
        var provider = new SystemDateTimeProvider();
        var before = DateTime.UtcNow;

        // Act
        var result = provider.UtcNow;

        // Assert
        var after = DateTime.UtcNow;
        result.ShouldBeGreaterThanOrEqualTo(before);
        result.ShouldBeLessThanOrEqualTo(after);
    }

    /// <summary>
    /// Tests that UtcNow returns a DateTime with UTC kind, not Local or Unspecified.
    /// Input: None (property access).
    /// Expected: DateTime.Kind should be DateTimeKind.Utc.
    /// </summary>
    [Fact]
    public void UtcNow_WhenAccessed_ReturnsDateTimeWithUtcKind()
    {
        // Arrange
        var provider = new SystemDateTimeProvider();

        // Act
        var result = provider.UtcNow;

        // Assert
        result.Kind.ShouldBe(DateTimeKind.Utc);
    }

    /// <summary>
    /// Tests that UtcNow does not return the default DateTime value (year 0001).
    /// Input: None (property access).
    /// Expected: Returned value should not be default and should be greater than DateTime.MinValue.
    /// </summary>
    [Fact]
    public void UtcNow_WhenAccessed_DoesNotReturnDefaultDateTime()
    {
        // Arrange
        var provider = new SystemDateTimeProvider();

        // Act
        var result = provider.UtcNow;

        // Assert
        result.ShouldNotBe(default(DateTime));
        result.ShouldBeGreaterThan(DateTime.MinValue);
    }

    /// <summary>
    /// Tests that subsequent calls to UtcNow return non-decreasing time values, ensuring time progresses.
    /// Input: Multiple property accesses.
    /// Expected: Each subsequent call should return a time greater than or equal to the previous call.
    /// </summary>
    [Fact]
    public void UtcNow_WhenAccessedMultipleTimes_ReturnsNonDecreasingValues()
    {
        // Arrange
        var provider = new SystemDateTimeProvider();

        // Act
        var first = provider.UtcNow;
        var second = provider.UtcNow;
        var third = provider.UtcNow;

        // Assert
        second.ShouldBeGreaterThanOrEqualTo(first);
        third.ShouldBeGreaterThanOrEqualTo(second);
    }
}