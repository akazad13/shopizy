using Shouldly;
using Shopizy.Infrastructure.Services;
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
}
