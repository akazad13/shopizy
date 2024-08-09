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
        var Order = OrderFactory.CreateOrder();
        var query = ListOrdersQueryUtils.CreateQuery();
        _mockOrderRepository
            .Setup(c => c.GetOrdersAsync())
            .ReturnsAsync(() => [Order]);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsError.Should().BeFalse();
    }
}
