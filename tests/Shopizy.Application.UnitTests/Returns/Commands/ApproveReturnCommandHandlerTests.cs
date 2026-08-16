using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Returns.Commands.ApproveReturn;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.Entities;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Models;
using Shouldly;

namespace Shopizy.Application.UnitTests.Returns.Commands;

public class ApproveReturnCommandHandlerTests
{
    private readonly Mock<IReturnRequestRepository> _mockReturnRepo;
    private readonly Mock<IPaymentRepository> _mockPaymentRepo;
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<IUnitOfWork> _mockUoW;
    private readonly ApproveReturnCommandHandler _handler;

    public ApproveReturnCommandHandlerTests()
    {
        _mockReturnRepo = new Mock<IReturnRequestRepository>();
        _mockPaymentRepo = new Mock<IPaymentRepository>();
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockPaymentService = new Mock<IPaymentService>();
        _mockUoW = new Mock<IUnitOfWork>();

        _handler = new ApproveReturnCommandHandler(
            _mockReturnRepo.Object,
            _mockPaymentRepo.Object,
            _mockOrderRepo.Object,
            _mockPaymentService.Object,
            _mockUoW.Object
        );
    }

    private static ReturnRequest CreatePendingReturn()
    {
        var orderId = OrderId.Create(Guid.NewGuid());
        var items = new List<ReturnItem>
        {
            ReturnItem.Create(OrderItemId.Create(Guid.NewGuid()), 1),
        };
        return ReturnRequest.Create(orderId, UserId.Create(Guid.NewGuid()), "Defective", items);
    }

    private static Payment CreatePaidPayment(OrderId orderId)
    {
        return Payment.Create(
            UserId.Create(Guid.NewGuid()),
            orderId,
            "Card",
            "pm_123",
            "ch_abc123",
            PaymentStatus.Payed,
            Price.CreateNew(50, Shopizy.Domain.Common.Enums.Currency.usd),
            Shopizy.Domain.Orders.ValueObjects.Address.CreateNew(
                "1 St",
                "City",
                "State",
                "US",
                "00000"
            )
        );
    }

    [Fact]
    public async Task Handle_WhenReturnNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new ApproveReturnCommand(Guid.NewGuid());
        _mockReturnRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<ReturnRequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReturnRequest?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenReturnApprovedAndRefundSucceeds_ShouldReturnSuccess()
    {
        // Arrange
        var returnRequest = CreatePendingReturn();
        var payment = CreatePaidPayment(returnRequest.OrderId);

        _mockReturnRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<ReturnRequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnRequest);
        _mockPaymentRepo
            .Setup(r => r.GetPaymentByOrderIdAsync(returnRequest.OrderId))
            .ReturnsAsync(payment);
        _mockPaymentService
            .Setup(s => s.CreateRefundAsync(payment.TransactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);
        _mockUoW.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new ApproveReturnCommand(returnRequest.Id.Value);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        _mockPaymentService.Verify(
            s => s.CreateRefundAsync(payment.TransactionId, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPaymentInvalid_ShouldReturnValidationError()
    {
        // Arrange
        var returnRequest = CreatePendingReturn();

        _mockReturnRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<ReturnRequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnRequest);
        _mockPaymentRepo
            .Setup(r => r.GetPaymentByOrderIdAsync(returnRequest.OrderId))
            .ReturnsAsync((Payment?)null);

        var command = new ApproveReturnCommand(returnRequest.Id.Value);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Validation);
    }
}
