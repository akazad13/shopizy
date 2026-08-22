using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Queries.GetProductVariants;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Products.Queries.GetProductVariants;

public class GetProductVariantsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly GetProductVariantsQueryHandler _handler;

    public GetProductVariantsQueryHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new GetProductVariantsQueryHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockProductRepository
            .Setup(r => r.GetProductByIdAsync(It.Is<ProductId>(id => id.Value == productId)))
            .ReturnsAsync((Product?)null);

        var query = new GetProductVariantsQuery(productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Product.ProductNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnVariants()
    {
        // Arrange
        var product = Product.Create(
            "Test",
            "Short",
            "Long",
            CategoryId.CreateUnique(),
            "SKU",
            10,
            Price.CreateNew(10, Currency.usd),
            null,
            null,
            "Barcode",
            "Colors",
            "Sizes",
            "Tags"
        );
        var variant = ProductVariant.Create(
            "Red-M",
            "SKU-RED-M",
            Price.CreateNew(12, Currency.usd),
            5
        );
        product.AddVariant(variant);

        _mockProductRepository
            .Setup(r => r.GetProductByIdAsync(It.Is<ProductId>(id => id.Value == product.Id.Value)))
            .ReturnsAsync(product);

        var query = new GetProductVariantsQuery(product.Id.Value);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Name.ShouldBe("Red-M");
    }
}
