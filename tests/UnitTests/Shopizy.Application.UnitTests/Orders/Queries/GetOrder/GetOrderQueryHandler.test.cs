using FluentAssertions;
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
    public async Task ShouldReturnErrorResponseWhenRequestedOrderDoesNotExist()
    {
        // Arrange
        var orderId = OrderId.CreateUnique();
        var query = GetOrderQueryUtils.CreateQuery();
        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().BeEquivalentTo(CustomErrors.Order.OrderNotFound);
    }

    [Fact]
    public async Task ShouldReturnCorrectOrderWhenRequestedOrderExists()
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
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Order));
        result.Value.Should().BeEquivalentTo(order);
    }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsWithoutDataInconsistencies()
    // {
    //     // Arrange
    //     var orderId = OrderId.Create(1);
    //     _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(orderId))
    //         .ReturnsAsync((Order)null)
    //         .Callback(() => Thread.Sleep(100)); // Simulate concurrent request

    //     var tasks = new List<Task<ErrorOr<Order>>>();

    //     for (int i = 0; i < 10; i++) // Simulate 10 concurrent requests
    //     {
    //         tasks.Add(_sut.Handle(new GetOrderQuery { OrderId = orderId.Value }, CancellationToken.None));
    //     }

    //     // Act
    //     await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var task in tasks)
    //     {
    //         Assert.IsType<ErrorResult<Order>>(task.Result);
    //         var errorResult = (ErrorResult<Order>)task.Result;
    //         Assert.Equal(CustomErrors.Order.OrderNotFound, errorResult.Errors.First());
    //     }
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenOrderIdIsNotProvided()
    // {
    //     // Arrange
    //     var orderId = default(int);
    //     _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(It.IsAny<OrderId>())).ReturnsAsync((Order)null);

    //     // Act
    //     var result = await _sut.Handle(
    //         new GetOrderQuery { OrderId = orderId },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<ErrorResult<Order>>(result);
    //     var errorResult = (ErrorResult<Order>)result;
    //     Assert.Equal(CustomErrors.Order.OrderNotFound, errorResult.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenOrderIdIsInvalid()
    // {
    //     // Arrange
    //     var invalidOrderId = 0;
    //     _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(It.IsAny<OrderId>())).ReturnsAsync((Order)null);

    //     // Act
    //     var result = await _sut.Handle(
    //         new GetOrderQuery { OrderId = invalidOrderId },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<ErrorResult<Order>>(result);
    //     var errorResult = (ErrorResult<Order>)result;
    //     Assert.Equal(CustomErrors.Order.OrderNotFound, errorResult.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldHandlePaginationForALargeNumberOfOrders()
    // {
    //     // Arrange
    //     var orderId1 = OrderId.Create(1);
    //     var orderId2 = OrderId.Create(2);
    //     var orderId3 = OrderId.Create(3);

    //     var orders = new List<Order>
    //     {
    //         new Order(orderId1, new CustomerId(1), new List<OrderItem>()),
    //         new Order(orderId2, new CustomerId(2), new List<OrderItem>()),
    //         new Order(orderId3, new CustomerId(3), new List<OrderItem>())
    //     };

    //     _mockOrderRepository.Setup(x => x.GetOrdersAsync(It.IsAny<int>(), It.IsAny<int>()))
    //         .ReturnsAsync(orders);

    //     // Act
    //     var result = await _sut.Handle(
    //         new GetOrderQuery { PageNumber = 1, PageSize = 2 },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<SuccessResult<List<Order>>>(result);
    //     var successResult = (SuccessResult<List<Order>>)result;
    //     Assert.Equal(2, successResult.Data.Count);
    //     Assert.Equal(orderId1, successResult.Data[0].OrderId);
    //     Assert.Equal(orderId2, successResult.Data[1].OrderId);
    // }

    // [Fact]
    // public async Task ShouldReturnEmptyListWhenNoOrdersAreFound()
    // {
    //     // Arrange
    //     var orderId = OrderId.Create(1);
    //     _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

    //     // Act
    //     var result = await _sut.Handle(
    //         new GetOrderQuery { OrderId = orderId.Value },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<SuccessResult<Order>>(result);
    //     var successResult = (SuccessResult<Order>)result;
    //     Assert.Null(successResult.Data);
    // }

    // [Fact]
    // public void ShouldValidateInputParametersToPreventSqlInjectionAttacks()
    // {
    //     // Arrange
    //     var orderId = "1"; // Potential SQL injection input
    //     var expectedErrorMessage = CustomErrors.Order.InvalidOrderId;

    //     // Act
    //     var result = await _sut.Handle(
    //         new GetOrderQuery { OrderId = orderId },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     Assert.IsType<ErrorResult<Order>>(result);
    //     var errorResult = (ErrorResult<Order>)result;
    //     Assert.Equal(expectedErrorMessage, errorResult.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldHandleALargeVolumeOfRequestsWithoutPerformanceDegradation()
    // {
    //     // Arrange
    //     var orderId = OrderId.Create(1);
    //     _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(orderId)).ReturnsAsync(new Order(orderId, new List<OrderItem>()));

    //     var tasks = new List<Task<ErrorOr<Order>>>();
    //     for (int i = 0; i < 1000; i++)
    //     {
    //         tasks.Add(_sut.Handle(new GetOrderQuery { OrderId = orderId.Value }, CancellationToken.None));
    //     }

    //     // Act
    //     var results = await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var result in results)
    //     {
    //         Assert.IsType<SuccessResult<Order>>(result);
    //         var successResult = (SuccessResult<Order>)result;
    //         Assert.NotNull(successResult.Data);
    //     }
    // }


    //     [Fact]
    //     public void ShouldEnsureOrderRepositoryIsProperlyDisposedAfterUse()
    //     {
    //         // Arrange
    //         var orderRepositoryMock = new Mock<IOrderRepository>();
    //         var sut = new GetOrderQueryHandler(orderRepositoryMock.Object);

    //         // Act
    //         sut.Handle(
    //             new GetOrderQuery { OrderId = 1 },
    //             CancellationToken.None
    //         );

    //         // Assert
    //         orderRepositoryMock.Verify(x => x.Dispose(), Times.Once);
    //     }
}
