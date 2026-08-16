using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.BulkUpdateProductStatus;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.BulkUpdateProductStatus;

public class BulkUpdateProductStatusCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly BulkUpdateProductStatusCommandHandler _sut;

    public BulkUpdateProductStatusCommandHandlerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _sut = new BulkUpdateProductStatusCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_UpdateProductStatus_WhenProductsExist()
    {
        // Arrange
        var p1 = ProductFactory.CreateProduct();
        var p2 = ProductFactory.CreateProduct();
        var command = new BulkUpdateProductStatusCommand([p1.Id.Value, p2.Id.Value], false);

        _mockRepo
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync([p1, p2]);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Success, result.Value);
        Assert.False(p1.IsActive);
        Assert.False(p2.IsActive);
    }
}
