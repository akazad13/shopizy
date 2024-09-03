using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Queries.ListOrders;
using Shopizy.Application.UnitTests.Orders.TestUtils;

namespace Shopizy.Application.UnitTests.Orders.Queries.ListOrders;

public class ListOrdersQueryHandlerTests
{
    private readonly ListOrdersQueryHandler _handler;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public ListOrdersQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _handler = new ListOrdersQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task ListOrders_WhenEverythingOkay_ReturnOrderList()
    {
        // Arrange
        Domain.Orders.Order Order = OrderFactory.CreateOrder();
        ListOrdersQuery query = ListOrdersQueryUtils.CreateQuery();
        _ = _mockOrderRepository
            .Setup(c => c.GetOrdersAsync())
            .ReturnsAsync(() => [Order]);

        // Act
        ErrorOr.ErrorOr<List<Domain.Orders.Order>> result = await _handler.Handle(query, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
    }
}
