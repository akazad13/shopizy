using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.RemoveVariant;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.RemoveVariant;

public class RemoveVariantCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly RemoveVariantCommandHandler _sut;

    public RemoveVariantCommandHandlerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _sut = new RemoveVariantCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new RemoveVariantCommand(Guid.NewGuid(), Guid.NewGuid());

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
    public async Task Should_RemoveVariant_WhenProductAndVariantExist()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var variant = ProductVariant.Create(
            "Blue / M",
            "SKU-BLUE-M",
            Shopizy.Domain.Common.ValueObjects.Price.CreateNew(19.99m, Currency.usd),
            5
        );
        product.AddVariant(variant);

        var command = new RemoveVariantCommand(product.Id.Value, variant.Id.Value);

        _mockRepo.Setup(x => x.GetProductByIdForUpdateAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Deleted, result.Value);
    }

    [Fact]
    public async Task Should_ReturnVariantNotFound_WhenVariantDoesNotExistInProduct()
    {
        var product = ProductFactory.CreateProduct();
        var nonExistentVariantId = Guid.NewGuid();
        var command = new RemoveVariantCommand(product.Id.Value, nonExistentVariantId);

        _mockRepo.Setup(x => x.GetProductByIdForUpdateAsync(product.Id)).ReturnsAsync(product);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.ProductVariant.VariantNotFound, result.FirstError);
    }
}
