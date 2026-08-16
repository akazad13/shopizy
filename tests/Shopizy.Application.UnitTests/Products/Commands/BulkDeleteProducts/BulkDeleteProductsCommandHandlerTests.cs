using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.BulkDeleteProducts;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.BulkDeleteProducts;

public class BulkDeleteProductsCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly BulkDeleteProductsCommandHandler _sut;

    public BulkDeleteProductsCommandHandlerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _sut = new BulkDeleteProductsCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_WhenNoProductsMatch()
    {
        // Arrange
        var command = new BulkDeleteProductsCommand([Guid.NewGuid()]);

        _mockRepo.Setup(x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>())).ReturnsAsync([]);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Product.ProductNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_DeleteProducts_WhenProductsExist()
    {
        // Arrange
        var p1 = ProductFactory.CreateProduct();
        var p2 = ProductFactory.CreateProduct();
        var command = new BulkDeleteProductsCommand([p1.Id.Value, p2.Id.Value]);

        _mockRepo
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync([p1, p2]);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Deleted, result.Value);
        _mockRepo.Verify(x => x.RemoveRange(It.IsAny<IList<Product>>()), Times.Once);
    }
}
