using FluentAssertions;
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
    public async Task ShouldReturnNotFoundWhenNoOrdersAreFound()
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
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().BeEquivalentTo(CustomErrors.Order.OrderNotFound);
    }

    [Fact]
    public async Task ReturnOrderListWhenEverythingOkay()
    {
        // Arrange
        var Order = OrderFactory.CreateOrder();
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
            .ReturnsAsync(() => [Order]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<List<OrderDto>>();
        result.Value.Should().NotBeNullOrEmpty();
    }

    // [Fact]
    // public async Task ShouldHandleALargeNumberOfOrdersEfficiently()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var largeNumberOfOrders = new List<Order>(Enumerable.Repeat(new Order(), 10001));
    //     mockOrderRepository.Setup(repo => repo.GetOrdersAsync()).ReturnsAsync(largeNumberOfOrders);

    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(largeNumberOfOrders.Count, result.Data.Count);
    // }

    // [Fact]
    // public async Task ShouldReturnSortedListOfOrdersBasedOnCreationDate()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var unsortedOrders = new List<Order>
    //     {
    //         new Order { Id = 1, CreatedDate = new DateTime(2022, 1, 1) },
    //         new Order { Id = 2, CreatedDate = new DateTime(2022, 12, 31) },
    //         new Order { Id = 3, CreatedDate = new DateTime(2022, 6, 15) },
    //     };
    //     mockOrderRepository.Setup(repo => repo.GetOrdersAsync()).ReturnsAsync(unsortedOrders);

    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(
    //         unsortedOrders.OrderBy(o => o.CreatedDate).Select(o => o.Id).ToList(),
    //         result.Data.Select(o => o.Id).ToList()
    //     );
    // }

    // [Fact]
    // public async Task ShouldFilterOrdersBasedOnASpecificCustomerID()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var orders = new List<Order>
    //     {
    //         new Order { Id = 1, CustomerId = 1 },
    //         new Order { Id = 2, CustomerId = 2 },
    //         new Order { Id = 3, CustomerId = 1 },
    //     };
    //     mockOrderRepository.Setup(repo => repo.GetOrdersAsync()).ReturnsAsync(orders);

    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new GetOrdersQuery { CustomerId = 1 },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(2, result.Data.Count);
    //     Assert.All(result.Data, order => Assert.Equal(1, order.CustomerId));
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsWithoutDataCorruption()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     var tasks = new List<Task<ErrorOr<List<Order>>>>();

    //     for (int i = 0; i < 10; i++)
    //     {
    //         tasks.Add(handler.Handle(new GetOrdersQuery(), CancellationToken.None));
    //     }

    //     // Act
    //     var results = await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var result in results)
    //     {
    //         Assert.IsType<Response<List<Order>>>(result);
    //         Assert.True(result.IsSuccess);
    //         Assert.NotNull(result.Data);
    //     }
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenDatabaseConnectionFails()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     mockOrderRepository
    //         .Setup(repo => repo.GetOrdersAsync())
    //         .ThrowsAsync(new DbException("Database connection failed"));

    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Null(result.Data);
    //     Assert.Equal("Database connection failed", result.ErrorMessage);
    // }

    // [Fact]
    // public void ShouldValidateInputParametersForSecurityReasons()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = handler.Handle(null, CancellationToken.None);

    //     // Assert
    //     Assert.ThrowsAsync<ArgumentNullException>(() => result);
    // }

    // [Fact]
    // public async Task ShouldHandlePaginationWhenRetrievingALargeNumberOfOrders()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var largeNumberOfOrders = new List<Order>(Enumerable.Repeat(new Order(), 10001));
    //     mockOrderRepository
    //         .Setup(repo => repo.GetOrdersAsync(It.IsAny<int>(), It.IsAny<int>()))
    //         .ReturnsAsync(
    //             (int pageNumber, int pageSize) =>
    //                 largeNumberOfOrders.Skip(pageNumber * pageSize).Take(pageSize).ToList()
    //         );

    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new GetOrdersQuery { PageNumber = 0, PageSize = 1000 },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(1000, result.Data.Count);
    // }

    // [Fact]
    // public async Task ShouldSupportSearchingForOrdersBySpecificKeywords()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var ordersWithKeyword = new List<Order>
    //     {
    //         new Order { Id = 1, Description = "Keyword Order 1" },
    //         new Order { Id = 2, Description = "Non-matching Order 2" },
    //         new Order { Id = 3, Description = "Keyword Order 3" },
    //     };
    //     mockOrderRepository
    //         .Setup(repo => repo.GetOrdersAsync(It.Is<string>(s => s == "Keyword")))
    //         .ReturnsAsync(ordersWithKeyword);

    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new GetOrdersQuery { Keyword = "Keyword" },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(2, result.Data.Count);
    //     Assert.All(result.Data, order => Assert.Contains("Keyword", order.Description));
    // }

    // [Fact]
    // public async Task ShouldReturnPaginatedListWhenPageSizeIsExceeded()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var allOrders = new List<Order>(Enumerable.Repeat(new Order(), 100));
    //     mockOrderRepository
    //         .Setup(repo => repo.GetOrdersAsync(It.IsAny<int>(), It.IsAny<int>()))
    //         .ReturnsAsync(
    //             (int page, int pageSize) => allOrders.Skip(page * pageSize).Take(pageSize).ToList()
    //         );

    //     var handler = new GetOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new GetOrdersQuery { PageSize = 50 },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(50, result.Data.Count);
    // }
}
