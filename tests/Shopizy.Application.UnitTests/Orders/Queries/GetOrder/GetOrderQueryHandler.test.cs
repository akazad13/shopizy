using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Queries.GetOrder;

public class GetOrderQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetOrderQueryHandler _sut;

    public GetOrderQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new GetOrderQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnErrorResponse_WhenRequestedOrderDoesNotExist()
    {
        // Arrange
        var orderId = OrderId.CreateUnique();
        var query = GetOrderQueryUtils.CreateQuery();
        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Order.OrderNotFound, result.Errors[0]);
    }

    [Fact]
    public async Task Should_ReturnCorrectOrder_WhenRequestedOrderExists()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var query = GetOrderQueryUtils.CreateQuery();

        _mockOrderRepository
            .Setup(x => x.GetOrderByIdAsync(OrderId.Create(query.OrderId)))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.IsType<Order>(result.Value);
        Assert.Equal(order, result.Value);
    }
}
