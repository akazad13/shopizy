using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Payments.Queries.GetPaymentByOrder;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Queries;

public class GetPaymentByOrderQueryHandlerTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly GetPaymentByOrderQueryHandler _handler;

    public GetPaymentByOrderQueryHandlerTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _handler = new GetPaymentByOrderQueryHandler(_mockPaymentRepository.Object);
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
    public async Task Handle_WhenPaymentExistsForOrder_ShouldReturnPayment()
    {
        // Arrange
        var payment = CreateDummyPayment();
        var query = new GetPaymentByOrderQuery(payment.OrderId.Value);

        _mockPaymentRepository
            .Setup(r =>
                r.GetPaymentByOrderIdAsync(It.Is<OrderId>(id => id.Value == payment.OrderId.Value))
            )
            .ReturnsAsync(payment);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(payment);
    }

    [Fact]
    public async Task Handle_WhenPaymentDoesNotExistForOrder_ShouldReturnNotFoundError()
    {
        // Arrange
        var query = new GetPaymentByOrderQuery(Guid.NewGuid());

        _mockPaymentRepository
            .Setup(r => r.GetPaymentByOrderIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Payment?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    }
}
