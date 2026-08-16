using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Payments.Queries.GetPaymentHistory;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Queries;

public class GetPaymentHistoryQueryHandlerTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly GetPaymentHistoryQueryHandler _handler;

    public GetPaymentHistoryQueryHandlerTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _handler = new GetPaymentHistoryQueryHandler(_mockPaymentRepository.Object);
    }

    private static Payment CreateDummyPayment(UserId userId)
    {
        return Payment.Create(
            userId,
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
    public async Task Handle_WhenPaymentsExistForUser_ShouldReturnPayments()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var payments = new List<Payment> { CreateDummyPayment(userId), CreateDummyPayment(userId) };

        var query = new GetPaymentHistoryQuery(userId.Value);

        _mockPaymentRepository
            .Setup(r => r.GetPaymentsByUserIdAsync(It.Is<UserId>(id => id.Value == userId.Value)))
            .ReturnsAsync(payments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(payments.AsReadOnly());
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WhenNoPaymentsExistForUser_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetPaymentHistoryQuery(Guid.NewGuid());

        _mockPaymentRepository
            .Setup(r => r.GetPaymentsByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(new List<Payment>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEmpty();
    }
}
