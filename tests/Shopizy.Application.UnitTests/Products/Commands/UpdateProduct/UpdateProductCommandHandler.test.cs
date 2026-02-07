using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests
{
    private readonly UpdateProductCommandHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public UpdateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new UpdateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Should_UpdateTheProductSuccessfully_WhenAllRequiredFieldsAreProvided()
    {
        // Arrange
        var command = UpdateProductCommandUtils.CreateCommand();
        var product = ProductFactory.CreateProduct();

        _mockProductRepository
            .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));

        // Act
        var result = await _sut.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockProductRepository.Verify(
            x => x.GetProductByIdAsync(It.IsAny<ProductId>()),
            Times.Once
        );
        _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_When_ProductDoesNotExist()
    {
        // Arrange
        var command = UpdateProductCommandUtils.CreateCommand();

        _mockProductRepository
            .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(Shopizy.Domain.Common.CustomErrors.CustomErrors.Product.ProductNotFound, result.FirstError);

        _mockProductRepository.Verify(
            x => x.GetProductByIdAsync(It.IsAny<ProductId>()),
            Times.Once
        );
        _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    }
}
