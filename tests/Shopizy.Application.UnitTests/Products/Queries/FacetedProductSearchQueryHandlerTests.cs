using Moq;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Queries.FacetedSearch;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Products.Queries;

public class FacetedProductSearchQueryHandlerTests
{
    private readonly Mock<IProductSearchEngine> _mockSearchEngine = new();
    private readonly FacetedProductSearchQueryHandler _sut;

    public FacetedProductSearchQueryHandlerTests()
    {
        _sut = new FacetedProductSearchQueryHandler(_mockSearchEngine.Object);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToSearchEngineAndReturnResults()
    {
        // Arrange
        var query = new FacetedProductSearchQuery(
            SearchTerm: "laptop",
            MinPrice: 500m,
            MaxPrice: 1500m,
            PageNumber: 1,
            PageSize: 10
        );

        var expectedResult = new ProductSearchResultDto(
            Items: [],
            TotalCount: 0,
            PageNumber: 1,
            PageSize: 10,
            TotalPages: 0,
            Facets: [],
            SuggestedKeywords: []
        );

        _mockSearchEngine
            .Setup(x =>
                x.SearchProductsAsync(
                    It.Is<ProductSearchQueryDto>(dto =>
                        dto.SearchTerm == "laptop" && dto.MinPrice == 500m && dto.MaxPrice == 1500m
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(expectedResult);
    }
}
