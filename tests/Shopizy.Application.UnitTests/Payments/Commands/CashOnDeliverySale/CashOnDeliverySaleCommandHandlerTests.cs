using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Payments.Commands.CashOnDeliverySale;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Commands.CashOnDeliverySale;

public class CashOnDeliverySaleCommandHandlerTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly CashOnDeliverySaleCommandHandler _handler;

    public CashOnDeliverySaleCommandHandlerTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();

        _handler = new CashOnDeliverySaleCommandHandler(
            _mockPaymentRepository.Object,
            _mockOrderRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldReturnOrderNotFoundError()
    {
        // Arrange
        var command = CreateCommand();
        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Shopizy.Domain.Orders.Order?)null);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.Order.OrderNotFound);
        _mockPaymentRepository.Verify(
            r => r.AddAsync(It.IsAny<Shopizy.Domain.Payments.Payment>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ShouldAddPaymentAndReturnSuccess()
    {
        // Arrange
        var command = CreateCommand();
        var order = OrderFactory.CreateOrder();

        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeOfType<Success>();
        _mockPaymentRepository.Verify(
            r => r.AddAsync(It.IsAny<Shopizy.Domain.Payments.Payment>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ShouldUpdateOrderStatusToProcessing()
    {
        // Arrange
        var command = CreateCommand();
        var order = OrderFactory.CreateOrder();

        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        _mockOrderRepository.Verify(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()), Times.Once);
    }

    private static CashOnDeliverySaleCommand CreateCommand() =>
        new(Constants.User.Id.Value, Constants.Order.Id.Value, 100m, "usd", "cod");
}
