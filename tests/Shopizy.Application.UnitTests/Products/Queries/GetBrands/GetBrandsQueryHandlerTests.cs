using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Queries.GetBrands;

namespace Shopizy.Application.UnitTests.Products.Queries.GetBrands;

public class GetBrandsQueryHandlerTests
{
    private readonly Mock<IProductReader> _mockReader;
    private readonly GetBrandsQueryHandler _sut;

    public GetBrandsQueryHandlerTests()
    {
        _mockReader = new Mock<IProductReader>();
        _sut = new GetBrandsQueryHandler(_mockReader.Object);
    }

    [Fact]
    public async Task Should_ReturnBrandNamesList_WhenQueryIsHandled()
    {
        // Arrange
        var query = new GetBrandsQuery();
        IReadOnlyList<string> expectedBrands = ["Nike", "Adidas", "Puma"];

        _mockReader
            .Setup(x => x.GetBrandNamesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBrands);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(3, result.Value.Count);
        Assert.Contains("Nike", result.Value);
        Assert.Equal("products:brands", query.CacheKey);
        Assert.Equal(TimeSpan.FromMinutes(60), query.Expiration);
    }
}
