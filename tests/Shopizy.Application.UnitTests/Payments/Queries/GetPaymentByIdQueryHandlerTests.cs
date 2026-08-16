using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Payments.Queries.GetPaymentById;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Queries;

public class GetPaymentByIdQueryHandlerTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly GetPaymentByIdQueryHandler _handler;

    public GetPaymentByIdQueryHandlerTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _handler = new GetPaymentByIdQueryHandler(_mockPaymentRepository.Object);
    }

    private static Payment CreateDummyPayment()
    {
        return Payment.Create(
            UserId.Create(Guid.NewGuid()),
            OrderId.Create(Guid.NewGuid()),
            "Card",
            "pm_123",
            "ch_123",
            PaymentStatus.Payed,
            Price.CreateNew(100, Shopizy.Domain.Common.Enums.Currency.usd),
            Address.CreateNew("123 Main St", "City", "State", "US", "12345")
        );
    }

    [Fact]
    public async Task Handle_WhenPaymentExists_ShouldReturnPayment()
    {
        // Arrange
        var payment = CreateDummyPayment();
        var query = new GetPaymentByIdQuery(payment.Id.Value);

        _mockPaymentRepository
            .Setup(r => r.GetPaymentByIdAsync(It.Is<PaymentId>(id => id.Value == payment.Id.Value)))
            .ReturnsAsync(payment);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(payment);
    }

    [Fact]
    public async Task Handle_WhenPaymentDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var query = new GetPaymentByIdQuery(Guid.NewGuid());

        _mockPaymentRepository
            .Setup(r => r.GetPaymentByIdAsync(It.IsAny<PaymentId>()))
            .ReturnsAsync((Payment?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    }
}
