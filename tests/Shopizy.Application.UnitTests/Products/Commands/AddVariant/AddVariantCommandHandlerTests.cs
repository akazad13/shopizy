using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.AddVariant;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.AddVariant;

public class AddVariantCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly AddVariantCommandHandler _sut;

    public AddVariantCommandHandlerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _sut = new AddVariantCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new AddVariantCommand(
            Guid.NewGuid(),
            "Red / XL",
            "SKU-RED-XL",
            29.99m,
            Currency.usd,
            10
        );

        _mockRepo
            .Setup(x => x.GetProductByIdForUpdateAsync(It.IsAny<ProductId>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Product.ProductNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_AddVariant_WhenProductExists()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var command = new AddVariantCommand(
            product.Id.Value,
            "Red / XL",
            "SKU-RED-XL",
            29.99m,
            Currency.usd,
            10
        );

        _mockRepo.Setup(x => x.GetProductByIdForUpdateAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("Red / XL", result.Value.Name);
        Assert.Equal("SKU-RED-XL", result.Value.SKU);
    }
}
