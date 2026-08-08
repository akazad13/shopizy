using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Payments;

public class PaymentTests
{
    [Fact]
    public void Create_AndUpdateStatusAndTransaction_ShouldWork()
    {
        var userId = UserId.CreateUnique();
        var orderId = OrderId.CreateUnique();
        var price = Price.CreateNew(100, Currency.usd);
        var address = Address.CreateNew("Street", "City", "State", "Country", "12345");

        var payment = Payment.Create(
            userId,
            orderId,
            "Stripe",
            "pm_123",
            "tx_123",
            PaymentStatus.Pending,
            price,
            address
        );

        payment.ShouldNotBeNull();
        payment.UserId.ShouldBe(userId);
        payment.OrderId.ShouldBe(orderId);
        payment.PaymentStatus.ShouldBe(PaymentStatus.Pending);
        payment.TransactionId.ShouldBe("tx_123");

        payment.UpdatePaymentStatus(PaymentStatus.Payed);
        payment.PaymentStatus.ShouldBe(PaymentStatus.Payed);

        payment.UpdateTransactionId("tx_456");
        payment.TransactionId.ShouldBe("tx_456");
    }

    [Fact]
    public void Complete_ShouldSetStatusAndAddDomainEvent()
    {
        var userId = UserId.CreateUnique();
        var orderId = OrderId.CreateUnique();
        var price = Price.CreateNew(100, Currency.usd);
        var address = Address.CreateNew("Street", "City", "State", "Country", "12345");

        var payment = Payment.Create(
            userId,
            orderId,
            "Stripe",
            "pm_123",
            "tx_123",
            PaymentStatus.Pending,
            price,
            address
        );

        payment.Complete("ch_123", "cus_123");

        payment.PaymentStatus.ShouldBe(PaymentStatus.Payed);
        payment.TransactionId.ShouldBe("ch_123");
        payment.DomainEvents.ShouldNotBeEmpty();
    }

    [Fact]
    public void PaymentId_CreateUniqueAndCreate_ShouldInitialize()
    {
        var pId1 = PaymentId.CreateUnique();
        var raw = Guid.NewGuid();
        var pId2 = PaymentId.Create(raw);

        pId1.Value.ShouldNotBe(Guid.Empty);
        pId2.Value.ShouldBe(raw);
    }
}
