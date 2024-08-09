using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Queries.GetOrder;

public class GetOrderQueryHandlerTests
{
    private readonly GetOrderQueryHandler _handler;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public GetOrderQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _handler = new GetOrderQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task GetOrder_WhenOrderIsFound_ReturnOrder()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var query = GetOrderQueryUtils.CreateQuery();
        _mockOrderRepository
            .Setup(c => c.GetOrderByIdAsync(OrderId.Create(query.OrderId)))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsError.Should().BeFalse();
    }
}
