using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Payments.Events;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Events;

public class PaymentSucceededDomainEventHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PaymentSucceededDomainEventHandler _handler;

    public PaymentSucceededDomainEventHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new PaymentSucceededDomainEventHandler(
            _mockOrderRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldReturnWithoutSavingChanges()
    {
        // Arrange
        var domainEvent = new PaymentSucceededDomainEvent(
            OrderId.Create(Guid.NewGuid()),
            "cus_123"
        );

        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Shopizy.Domain.Orders.Order?)null);

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOrderFound_ShouldCompletePaymentAndSaveChanges()
    {
        // Arrange
        const string customerId = "cus_789";
        var order = OrderFactory.CreateOrder();
        var domainEvent = new PaymentSucceededDomainEvent(order.Id, customerId);

        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync(order);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _mockOrderRepository.Verify(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
