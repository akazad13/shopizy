using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Queries.ListOrders;

public class GetOrdersQueryHandlerTests
{
    private readonly GetOrdersQueryHandler _sut;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public GetOrdersQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new GetOrdersQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenNoOrdersAreFound()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var query = GetOrdersQueryUtils.CreateQuery();

        var customerId = query.CustomerId is null ? null : UserId.Create(query.CustomerId.Value);

        mockOrderRepository
            .Setup(repo =>
                repo.GetOrdersAsync(
                    customerId,
                    query.StartDate,
                    query.EndDate,
                    query.Status,
                    query.PageNumber,
                    query.PageSize,
                    OrderType.Ascending
                )
            )
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Order.OrderNotFound, result.Errors[0]);
    }

    [Fact]
    public async Task Should_ReturnOrderList_WhenEverythingOkay()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var query = GetOrdersQueryUtils.CreateQuery();
        var customerId = query.CustomerId is null ? null : UserId.Create(query.CustomerId.Value);

        _mockOrderRepository
            .Setup(c =>
                c.GetOrdersAsync(
                    customerId,
                    query.StartDate,
                    query.EndDate,
                    query.Status,
                    query.PageNumber,
                    query.PageSize,
                    OrderType.Ascending
                )
            )
            .ReturnsAsync(new List<Order> { order });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<List<OrderDto>>(result.Value);
        Assert.NotNull(result.Value);
    }
}
