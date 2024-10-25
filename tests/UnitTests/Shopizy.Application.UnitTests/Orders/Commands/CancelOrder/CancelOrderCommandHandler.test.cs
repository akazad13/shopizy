using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly CancelOrderCommandHandler _sut;

    public CancelOrderCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new CancelOrderCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task ShouldReturnOrderIsNotFoundWhenOrderIdIsNotProvidedAsync()
    {
        // Arrange
        var command = CancelOrderCommandUtils.CreateCommand(Guid.Empty);

        // Act
        var result = (await _sut.Handle(command, CancellationToken.None)).Match(x => null, x => x);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(GenericResponse));
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().Be(CustomErrors.Order.OrderNotFound);

        _mockOrderRepository.Verify(
            x => x.GetOrderByIdAsync(OrderId.Create(command.OrderId)),
            Times.Once
        );
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Order>()), Times.Never);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShouldCancelTheOrderWhenOrderExistsAndCancellationReasonIsValidAsync()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var command = CancelOrderCommandUtils.CreateCommand(order.Id.Value);

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);
        _mockOrderRepository.Setup(x => x.Update(order));
        _mockOrderRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = (await _sut.Handle(command, CancellationToken.None)).Match(x => x, x => null);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(GenericResponse));
        result.Errors.Should().BeEmpty();
        result.Message.Should().Be("Successfully canceled the order.");

        _mockOrderRepository.Verify(
            x => x.GetOrderByIdAsync(OrderId.Create(command.OrderId)),
            Times.Once
        );
        _mockOrderRepository.Verify(x => x.Update(order), Times.Once);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Fact]
    // public async Task ShouldNotCancelTheOrderWhenOrderDoesNotExist()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = "Test Reason" };
    //     _orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync((Order)null);
    //     var handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.ShouldBeTrue();
    //     result.Errors.ShouldContain(CustomErrors.Order.OrderNotFound);
    //     _orderRepositoryMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Never);
    //     _orderRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldNotCancelTheOrderWhenCancellationReasonIsEmpty()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var order = new Order(OrderId.Create(orderId), "Test User"); // Assuming Order class with constructor and CancelOrder method
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = "" };
    //     _orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync(order);
    //     _orderRepositoryMock.Setup(x => x.Update(order));
    //     _orderRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Order.InvalidCancellationReason);
    //     _orderRepositoryMock.Verify(x => x.Update(order), Times.Never);
    //     _orderRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldNotCancelOrderWhenCancellationReasonExceedsMaxLength()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var order = new Order(OrderId.Create(orderId), "Test User"); // Assuming Order class with constructor and CancelOrder method
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = new string('a', 256) }; // Cancellation reason exceeds maximum length
    //     _orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync(order);
    //     _orderRepositoryMock.Setup(x => x.Update(order));
    //     _orderRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.ShouldBeTrue();
    //     result.Errors.ShouldContain(CustomErrors.Order.InvalidCancellationReason);
    //     _orderRepositoryMock.Verify(x => x.Update(order), Times.Never);
    //     _orderRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldNotUpdateOrderWhenCancellationFails()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var order = new Order(OrderId.Create(orderId), "Test User"); // Assuming Order class with constructor and CancelOrder method
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = "Test Reason" };
    //     _orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync(order);
    //     _orderRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);
    //     var handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.ShouldBeTrue();
    //     result.Errors.ShouldContain(CustomErrors.Order.OrderNotCancelled);
    //     _orderRepositoryMock.Verify(x => x.Update(order), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenCancellationFails()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var order = new Order(OrderId.Create(orderId), "Test User"); // Assuming Order class with constructor and CancelOrder method
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = "Test Reason" };
    //     _orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync(order);
    //     _orderRepositoryMock.Setup(x => x.Update(order));
    //     _orderRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0); // Simulate cancellation failure
    //     var handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.ShouldBeTrue();
    //     result.Errors.ShouldContain(CustomErrors.Order.OrderNotCancelled);
    // }

    // [Fact]
    // public async Task ShouldReturnSuccessResponseWhenCancellationIsSuccessful()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var order = new Order(OrderId.Create(orderId), "Test User"); // Assuming Order class with constructor and CancelOrder method
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = "Test Reason" };
    //     _orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync(order);
    //     _orderRepositoryMock.Setup(x => x.Update(order));
    //     _orderRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsSuccess.ShouldBeTrue();
    //     result.Value.Message.ShouldBe("Successfully canceled the order.");
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsByEnsuringDataConsistency()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var order = new Order(OrderId.Create(orderId), "Test User"); // Assuming Order class with constructor and CancelOrder method
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = "Test Reason" };
    //     var orderRepositoryMock = new Mock<IOrderRepository>();
    //     orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync(order);
    //     orderRepositoryMock.Setup(x => x.Update(order));
    //     orderRepositoryMock
    //         .Setup(x => x.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1)
    //         .Callback(() =>
    //         {
    //             // Simulate concurrent request
    //             var concurrentOrder = new Order(OrderId.Create(orderId), "Concurrent User");
    //             orderRepositoryMock
    //                 .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //                 .ReturnsAsync(concurrentOrder);
    //         });
    //     var handler = new CancelOrderCommandHandler(orderRepositoryMock.Object);

    //     // Act
    //     var result1 = await handler.Handle(command, CancellationToken.None);
    //     var result2 = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result1.IsSuccess.ShouldBeTrue();
    //     result1.Value.Message.ShouldBe("Successfully canceled the order.");
    //     result2.IsFailure.ShouldBeTrue();
    //     result2.Errors.ShouldContain(CustomErrors.Order.OrderNotCancelled);
    //     orderRepositoryMock.Verify(x => x.Update(order), Times.Once);
    //     orderRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Exactly(2));
    // }

    // [Fact]
    // public async Task ShouldHandleDatabaseConnectionFailures()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var order = new Order(OrderId.Create(orderId), "Test User"); // Assuming Order class with constructor and CancelOrder method
    //     var command = new CancelOrderCommand { OrderId = orderId, Reason = "Test Reason" };
    //     _orderRepositoryMock
    //         .Setup(x => x.GetOrderByIdAsync(OrderId.Create(orderId)))
    //         .ReturnsAsync(order);
    //     _orderRepositoryMock.Setup(x => x.Update(order));
    //     _orderRepositoryMock
    //         .Setup(x => x.Commit(It.IsAny<CancellationToken>()))
    //         .ThrowsAsync(new Exception("Database connection failed"));
    //     var handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.ShouldBeTrue();
    //     result.Errors.ShouldContain(CustomErrors.Order.DatabaseConnectionFailed);
    // }
}
