using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Payments.Commands.ProcessStripeWebhook;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Models;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Commands.ProcessStripeWebhook;

public class ProcessStripeWebhookCommandHandlerTests
{
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<ProcessStripeWebhookCommandHandler>> _mockLogger;
    private readonly ProcessStripeWebhookCommandHandler _handler;

    public ProcessStripeWebhookCommandHandlerTests()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<ProcessStripeWebhookCommandHandler>>();

        _handler = new ProcessStripeWebhookCommandHandler(
            _mockPaymentService.Object,
            _mockPaymentRepository.Object,
            _mockOrderRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handle_WhenParseFails_ShouldReturnError()
    {
        // Arrange
        var command = new ProcessStripeWebhookCommand("{}", "invalid_sig");
        _mockPaymentService
            .Setup(s => s.ParseWebhookEvent(command.JsonPayload, command.SignatureHeader))
            .Returns(Error.Validation("stripe.webhook_signature_invalid", "Invalid signature"));

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe("stripe.webhook_signature_invalid");
    }

    [Fact]
    public async Task Handle_WhenEventIsUnknown_ShouldReturnSuccess()
    {
        // Arrange
        var command = new ProcessStripeWebhookCommand("{}", "valid_sig");
        _mockPaymentService
            .Setup(s => s.ParseWebhookEvent(command.JsonPayload, command.SignatureHeader))
            .Returns(
                new PaymentWebhookEvent(
                    PaymentWebhookEventType.Unknown,
                    null,
                    null,
                    null,
                    null,
                    null
                )
            );

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPaymentSucceeded_ShouldCompletePaymentAndOrder()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var orderId = order.Id;
        var payment = Payment.Create(
            order.UserId,
            orderId,
            "Card",
            "pm_123",
            "",
            PaymentStatus.Pending,
            order.GetTotal(),
            order.ShippingAddress
        );

        var webhookEvent = new PaymentWebhookEvent(
            PaymentWebhookEventType.PaymentSucceeded,
            orderId.Value.ToString(),
            "ch_12345",
            "cus_67890",
            "pi_12345",
            null
        );

        var command = new ProcessStripeWebhookCommand("{}", "valid_sig");
        _mockPaymentService
            .Setup(s => s.ParseWebhookEvent(command.JsonPayload, command.SignatureHeader))
            .Returns(webhookEvent);

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

        _mockPaymentRepository
            .Setup(r => r.GetPaymentByOrderIdAsync(orderId))
            .ReturnsAsync(payment);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        payment.PaymentStatus.ShouldBe(PaymentStatus.Payed);
        payment.TransactionId.ShouldBe("ch_12345");
        order.OrderStatus.ShouldBe(OrderStatus.Processing);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPaymentFailed_ShouldCancelPendingPayment()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var orderId = order.Id;
        var payment = Payment.Create(
            order.UserId,
            orderId,
            "Card",
            "pm_123",
            "",
            PaymentStatus.Pending,
            order.GetTotal(),
            order.ShippingAddress
        );

        var webhookEvent = new PaymentWebhookEvent(
            PaymentWebhookEventType.PaymentFailed,
            orderId.Value.ToString(),
            null,
            "cus_67890",
            "pi_12345",
            "Card declined"
        );

        var command = new ProcessStripeWebhookCommand("{}", "valid_sig");
        _mockPaymentService
            .Setup(s => s.ParseWebhookEvent(command.JsonPayload, command.SignatureHeader))
            .Returns(webhookEvent);

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

        _mockPaymentRepository
            .Setup(r => r.GetPaymentByOrderIdAsync(orderId))
            .ReturnsAsync(payment);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        payment.PaymentStatus.ShouldBe(PaymentStatus.Cancelled);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenChargeRefunded_ShouldMarkPaymentRefundedAndCancelOrder()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        order.CompletePayment("cus_67890"); // OrderStatus -> Processing
        var orderId = order.Id;
        var payment = Payment.Create(
            order.UserId,
            orderId,
            "Card",
            "pm_123",
            "ch_12345",
            PaymentStatus.Payed,
            order.GetTotal(),
            order.ShippingAddress
        );

        var webhookEvent = new PaymentWebhookEvent(
            PaymentWebhookEventType.ChargeRefunded,
            orderId.Value.ToString(),
            "ch_12345",
            "cus_67890",
            "pi_12345",
            null
        );

        var command = new ProcessStripeWebhookCommand("{}", "valid_sig");
        _mockPaymentService
            .Setup(s => s.ParseWebhookEvent(command.JsonPayload, command.SignatureHeader))
            .Returns(webhookEvent);

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

        _mockPaymentRepository
            .Setup(r => r.GetPaymentByOrderIdAsync(orderId))
            .ReturnsAsync(payment);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        payment.PaymentStatus.ShouldBe(PaymentStatus.Refunded);
        order.OrderStatus.ShouldBe(OrderStatus.Cancelled);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
