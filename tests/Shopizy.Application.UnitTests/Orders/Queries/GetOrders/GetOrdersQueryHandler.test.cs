using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Orders.Queries.GetOrders;

public class GetOrdersQueryHandlerTestsRefactored
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTestsRefactored()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _handler = new GetOrdersQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnOrderDtos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetOrdersQuery(userId, null, null, null, 1, 10);

        var orders = new List<Order> { CreateSampleOrder(UserId.Create(userId)) };

        _mockOrderRepository
            .Setup(r => r.GetOrdersAsync(It.IsAny<UserId?>(), null, null, null, 1, 10))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
    }

    private static Order CreateSampleOrder(UserId userId) =>
        Order.Create(
            userId,
            "",
            (int)DeliveryMethods.Standard,
            Price.CreateNew(0, Currency.usd),
            Shopizy.Domain.Orders.ValueObjects.Address.CreateNew("S", "C", "ST", "CO", "Z"),
            new List<OrderItem>()
        );
}
