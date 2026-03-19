using Moq;
using Shouldly;
using Shopizy.Application.Products.Queries.GetProducts;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Products;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Queries.GetProducts;

public class GetProductsQueryHandlerTestsRefactored
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTestsRefactored()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new GetProductsQueryHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenProductsExist_ShouldReturnProductList()
    {
        // Arrange
        var query = new GetProductsQuery(null, null, null, null, 1, 10);
        var product = Product.Create(
            "Name", "Short", "Long", CategoryId.CreateUnique(), "SKU", 100,
            Price.CreateNew(10, Currency.usd), null, "B", "B", "C", "S", "T");
        var products = new List<Product> { product };

        _mockProductRepository.Setup(r => r.GetProductsAsync(
            It.IsAny<IReadOnlyList<ProductId>?>(),
            It.IsAny<string?>(),
            It.IsAny<IReadOnlyList<CategoryId>?>(),
            It.IsAny<decimal?>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
            .ReturnsAsync(products);

        _mockProductRepository.Setup(r => r.CountProductsAsync(
            It.IsAny<IReadOnlyList<ProductId>?>(),
            It.IsAny<string?>(),
            It.IsAny<IReadOnlyList<CategoryId>?>(),
            It.IsAny<decimal?>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Products.Count.ShouldBe(1);
        result.Value.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WhenNoProductsFound_ShouldReturnError()
    {
        // Arrange
        var query = new GetProductsQuery(null, null, null, null, 1, 10);

        _mockProductRepository.Setup(r => r.GetProductsAsync(
            It.IsAny<IReadOnlyList<ProductId>?>(),
            It.IsAny<string?>(),
            It.IsAny<IReadOnlyList<CategoryId>?>(),
            It.IsAny<decimal?>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
            .ReturnsAsync((IReadOnlyList<Product>?)null);

        _mockProductRepository.Setup(r => r.CountProductsAsync(
            It.IsAny<IReadOnlyList<ProductId>?>(),
            It.IsAny<string?>(),
            It.IsAny<IReadOnlyList<CategoryId>?>(),
            It.IsAny<decimal?>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.Product.ProductNotFound);
    }
}
