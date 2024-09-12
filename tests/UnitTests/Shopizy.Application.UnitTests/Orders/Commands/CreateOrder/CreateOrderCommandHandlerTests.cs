using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private readonly CreateOrderCommandHandler _handler;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public CreateOrderCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new CreateOrderCommandHandler(
            _mockProductRepository.Object,
            _mockOrderRepository.Object
        );
    }

    [Fact]
    public async Task CreateOrder_ProductExists_CreateOrderAndReturnSuccess()
    {
        // Arrange
        Domain.Products.Product product = ProductFactory.CreateProduct();
        CreateOrderCommand command = CreateOrderCommandUtils.CreateCommand(product.Id);

        _ = _mockProductRepository
            .Setup(
                x =>
                    x.GetProductsByIdsAsync(
                        It.IsAny<List<ProductId>>()
                    )
            )
            .ReturnsAsync(() => [product]);

        _ = _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>()));
        _ = _mockOrderRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        ErrorOr.ErrorOr<Order> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockProductRepository.Verify(
            x =>
                x.GetProductsByIdsAsync(
                    It.IsAny<List<ProductId>>()
                ),
            Times.Once
        );
        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        _ = result.IsError.Should().BeFalse();
        _ = result.Value.Should().BeOfType(typeof(Order));
    }
}
