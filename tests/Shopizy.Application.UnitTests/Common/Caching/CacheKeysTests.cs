using Shouldly;

namespace Shopizy.Application.UnitTests.Common.Caching;

public class CacheKeysTests
{
    [Fact]
    public void Product_ShouldReturnFormattedKey()
    {
        // Arrange
        var productId = Guid.Parse("11111111-2222-3333-4444-555555555555");

        // Act
        var key = CacheKeys.Product(productId);

        // Assert
        key.ShouldBe("product:11111111-2222-3333-4444-555555555555");
    }
}
