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
        Domain.Orders.Order order = OrderFactory.CreateOrder();
        GetOrderQuery query = GetOrderQueryUtils.CreateQuery();
        _ = _mockOrderRepository
            .Setup(c => c.GetOrderByIdAsync(OrderId.Create(query.OrderId)))
            .ReturnsAsync(order);

        // Act
        ErrorOr.ErrorOr<Domain.Orders.Order> result = await _handler.Handle(query, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
    }
}
