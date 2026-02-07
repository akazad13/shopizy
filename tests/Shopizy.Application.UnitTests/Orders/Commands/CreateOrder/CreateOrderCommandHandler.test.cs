using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly CreateOrderCommandHandler _sut;

    public CreateOrderCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _sut = new CreateOrderCommandHandler(
            _mockProductRepository.Object,
            _mockOrderRepository.Object,
            _mockCurrentUser.Object
        );
    }

    [Fact]
    public async Task Should_HandleValidOrderCreationRequest()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var command = CreateOrderCommandUtils.CreateCommand(new List<Guid> { product.Id.Value });

        _mockProductRepository
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync(() => [product]);

        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.IsType<Order>(result.Value);
        Assert.Single(result.Value.OrderItems);
        Assert.Equal(command.OrderItems.First().Quantity, result.Value.OrderItems[0].Quantity);

        _mockProductRepository.Verify(
            x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()),
            Times.Once
        );
        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnError_WhenProductIdNotFound()
    {
        // Arrange
        var command = CreateOrderCommandUtils.CreateCommand(new List<Guid>());

        _mockProductRepository
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors[0]);
    }
}
