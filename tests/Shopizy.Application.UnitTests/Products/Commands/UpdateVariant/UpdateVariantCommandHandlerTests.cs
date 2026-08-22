using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.UpdateVariant;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.UpdateVariant;

public class UpdateVariantCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly UpdateVariantCommandHandler _sut;

    public UpdateVariantCommandHandlerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _sut = new UpdateVariantCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new UpdateVariantCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Green / L",
            "SKU-G-L",
            35.00m,
            Currency.usd,
            20,
            true
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
    public async Task Should_UpdateVariant_WhenProductAndVariantExist()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var variant = ProductVariant.Create(
            "Green / L",
            "SKU-G-L",
            Shopizy.Domain.Common.ValueObjects.Price.CreateNew(30.00m, Currency.usd),
            15
        );
        product.AddVariant(variant);

        var command = new UpdateVariantCommand(
            product.Id.Value,
            variant.Id.Value,
            "Green / Large",
            "SKU-G-L-UPD",
            35.00m,
            Currency.usd,
            20,
            true
        );

        _mockRepo.Setup(x => x.GetProductByIdForUpdateAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("Green / Large", result.Value.Name);
        Assert.Equal(35.00m, result.Value.UnitPrice.Amount);
    }

    [Fact]
    public async Task Should_ReturnVariantNotFound_WhenVariantDoesNotExistInProduct()
    {
        var product = ProductFactory.CreateProduct();
        var command = new UpdateVariantCommand(
            product.Id.Value,
            Guid.NewGuid(),
            "Green / Large",
            "SKU-G-L-UPD",
            35.00m,
            Currency.usd,
            20,
            true
        );

        _mockRepo.Setup(x => x.GetProductByIdForUpdateAsync(product.Id)).ReturnsAsync(product);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.ProductVariant.VariantNotFound, result.FirstError);
    }
}
