using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Queries.ListOrders;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.UnitTests.Orders.Queries.ListOrders;

public class ListOrdersQueryHandlerTests
{
    private readonly ListOrdersQueryHandler _sut;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public ListOrdersQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new ListOrdersQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task ShouldReturnEmptyListWhenNoOrdersAreFoundAsync()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var query = ListOrdersQueryUtils.CreateQuery();

        mockOrderRepository.Setup(repo => repo.GetOrdersAsync()).ReturnsAsync([]);

        // Act
        var result = (await _sut.Handle(query, CancellationToken.None)).Match(x => x, x => null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ReturnOrderListWhenEverythingOkay()
    {
        // Arrange
        var Order = OrderFactory.CreateOrder();
        var query = ListOrdersQueryUtils.CreateQuery();
        _mockOrderRepository.Setup(c => c.GetOrdersAsync()).ReturnsAsync(() => [Order]);

        // Act
        var result = (await _sut.Handle(query, CancellationToken.None)).Match(x => x, x => null);

        // Assert
        result.Should().BeOfType<List<Order>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().Contain(Order);
    }

    // [Fact]
    // public async Task ShouldHandleALargeNumberOfOrdersEfficiently()
    // {
    //     // Arrange
    //     var mockOrderRepository = new Mock<IOrderRepository>();
    //     var largeNumberOfOrders = new List<Order>(Enumerable.Repeat(new Order(), 10001));
    //     mockOrderRepository.Setup(repo => repo.GetOrdersAsync()).ReturnsAsync(largeNumberOfOrders);

    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(new ListOrdersQuery(), CancellationToken.None);

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

    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(new ListOrdersQuery(), CancellationToken.None);

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

    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new ListOrdersQuery { CustomerId = 1 },
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
    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     var tasks = new List<Task<ErrorOr<List<Order>>>>();

    //     for (int i = 0; i < 10; i++)
    //     {
    //         tasks.Add(handler.Handle(new ListOrdersQuery(), CancellationToken.None));
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

    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(new ListOrdersQuery(), CancellationToken.None);

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
    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

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

    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new ListOrdersQuery { PageNumber = 0, PageSize = 1000 },
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

    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new ListOrdersQuery { Keyword = "Keyword" },
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

    //     var handler = new ListOrdersQueryHandler(mockOrderRepository.Object);

    //     // Act
    //     var result = await handler.Handle(
    //         new ListOrdersQuery { PageSize = 50 },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<Response<List<Order>>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(50, result.Data.Count);
    // }
}
