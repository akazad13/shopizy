using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Orders.Events;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Orders.Events;

public class OrderCancelledDomainEventHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<Shopizy.Application.Common.Interfaces.Services.IRealtimeNotifier> _mockRealtimeNotifier;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly OrderCancelledDomainEventHandler _sut;

    public OrderCancelledDomainEventHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockPaymentService = new Mock<IPaymentService>();
        _mockRealtimeNotifier =
            new Mock<Shopizy.Application.Common.Interfaces.Services.IRealtimeNotifier>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _sut = new OrderCancelledDomainEventHandler(
            _mockProductRepository.Object,
            _mockPaymentRepository.Object,
            _mockPaymentService.Object,
            _mockRealtimeNotifier.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Should_NotRefund_WhenPaymentIsNull()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var domainEvent = new OrderCancelledDomainEvent(order);

        _mockPaymentRepository
            .Setup(x => x.GetPaymentByOrderIdAsync(order.Id))
            .ReturnsAsync((Payment?)null);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockPaymentService.Verify(
            x => x.CreateRefundAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _mockPaymentRepository.Verify(x => x.Update(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public async Task Should_NotRefund_WhenPaymentIsNotPayed()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var payment = Payment.Create(
            Constants.User.Id,
            order.Id,
            "Card",
            "pm_123",
            "tx_123",
            PaymentStatus.Pending,
            Price.CreateNew(100, Currency.usd),
            Constants.User.Address
        );

        var domainEvent = new OrderCancelledDomainEvent(order);

        _mockPaymentRepository
            .Setup(x => x.GetPaymentByOrderIdAsync(order.Id))
            .ReturnsAsync(payment);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockPaymentService.Verify(
            x => x.CreateRefundAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _mockPaymentRepository.Verify(x => x.Update(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public async Task Should_NotRefund_WhenTransactionIdIsEmpty()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var payment = Payment.Create(
            Constants.User.Id,
            order.Id,
            "Card",
            "pm_123",
            "",
            PaymentStatus.Payed,
            Price.CreateNew(100, Currency.usd),
            Constants.User.Address
        );

        var domainEvent = new OrderCancelledDomainEvent(order);

        _mockPaymentRepository
            .Setup(x => x.GetPaymentByOrderIdAsync(order.Id))
            .ReturnsAsync(payment);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockPaymentService.Verify(
            x => x.CreateRefundAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _mockPaymentRepository.Verify(x => x.Update(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public async Task Should_NotUpdatePaymentStatus_WhenRefundServiceReturnsError()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var payment = Payment.Create(
            Constants.User.Id,
            order.Id,
            "Card",
            "pm_123",
            "tx_123",
            PaymentStatus.Payed,
            Price.CreateNew(100, Currency.usd),
            Constants.User.Address
        );

        var domainEvent = new OrderCancelledDomainEvent(order);

        _mockPaymentRepository
            .Setup(x => x.GetPaymentByOrderIdAsync(order.Id))
            .ReturnsAsync(payment);

        _mockPaymentService
            .Setup(x => x.CreateRefundAsync("tx_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.Failure("Refund.Failed", "Stripe error"));

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockPaymentRepository.Verify(x => x.Update(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public async Task Should_RefundAndSave_WhenPaymentIsPayedAndRefundSucceeds()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var payment = Payment.Create(
            Constants.User.Id,
            order.Id,
            "Card",
            "pm_123",
            "tx_123",
            PaymentStatus.Payed,
            Price.CreateNew(100, Currency.usd),
            Constants.User.Address
        );

        var domainEvent = new OrderCancelledDomainEvent(order);

        _mockPaymentRepository
            .Setup(x => x.GetPaymentByOrderIdAsync(order.Id))
            .ReturnsAsync(payment);

        _mockPaymentService
            .Setup(x => x.CreateRefundAsync("tx_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        Assert.Equal(PaymentStatus.Refunded, payment.PaymentStatus);
        _mockPaymentRepository.Verify(x => x.Update(payment), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_RestoreProductStock_WhenOrderIsCancelled()
    {
        // Arrange
        var product =
            Shopizy.Application.UnitTests.Products.TestUtils.ProductFactory.CreateProduct();
        var initialStock = product.StockQuantity;
        var orderItem = Shopizy.Domain.Orders.Entities.OrderItem.Create(
            product.Id,
            product.Name,
            "img.jpg",
            product.UnitPrice,
            3,
            "Red",
            "M",
            0
        );
        var order = Shopizy.Domain.Orders.Order.Create(
            Constants.User.Id,
            Constants.Order.PromoCode,
            Constants.Order.DeliveryMethod,
            Constants.Order.DeliveryCharge,
            Constants.User.Address,
            [orderItem]
        );

        _mockProductRepository
            .Setup(x =>
                x.GetProductsByIdsForUpdateAsync(
                    It.IsAny<IReadOnlyList<Shopizy.Domain.Products.ValueObjects.ProductId>>()
                )
            )
            .ReturnsAsync(new List<Shopizy.Domain.Products.Product> { product });

        var domainEvent = new OrderCancelledDomainEvent(order);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        product.StockQuantity.ShouldBe(initialStock + 3);
        _mockProductRepository.Verify(x => x.Update(product), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
