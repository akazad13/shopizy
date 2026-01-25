using Moq;
using Shouldly;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Common.Enums;

namespace Shopizy.Application.UnitTests.Products.Queries.GetProduct;

public class GetProductQueryHandlerTestsRefactored
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly GetProductQueryHandler _handler;

    public GetProductQueryHandlerTestsRefactored()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new GetProductQueryHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = Product.Create(
            "Name", "Short", "Long", CategoryId.CreateUnique(), "SKU", 100,
            Price.CreateNew(10, Currency.usd), null, "B", "B", "C", "S", "T");
        var query = new GetProductQuery(productId);

        _mockProductRepository.Setup(r => r.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(product);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var query = new GetProductQuery(Guid.NewGuid());

        _mockProductRepository.Setup(r => r.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.Product.ProductNotFound);
    }
}
