using Shopizy.Contracts.Payment;
using Shouldly;
using Xunit;

namespace Shopizy.Contracts.UnitTests;

// ─────────────────────────────────────────────────────────
// PaymentResponse
// ─────────────────────────────────────────────────────────
public class PaymentResponseTests
{
    private static PaymentResponse MakeSut(
        string chargeId = "ch_1A2B3C",
        string currency = "USD",
        long amount = 5000,
        string customerId = "cus_XYZ",
        string email = "alice@example.com",
        string description = "Order #42"
    ) => new(chargeId, currency, amount, customerId, email, description);

    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var sut = MakeSut(
            chargeId: "ch_TestCharge",
            currency: "GBP",
            amount: 12500,
            customerId: "cus_TestCust",
            email: "bob@example.com",
            description: "Payment for order 99"
        );

        sut.ChargeId.ShouldBe("ch_TestCharge");
        sut.Currency.ShouldBe("GBP");
        sut.Amount.ShouldBe(12500L);
        sut.CustomerId.ShouldBe("cus_TestCust");
        sut.ReceiptEmail.ShouldBe("bob@example.com");
        sut.Description.ShouldBe("Payment for order 99");
    }

    [Fact]
    public void Amount_ShouldBeStoredAsLong()
    {
        // Amounts in cents — ensure large values fit
        var largeAmount = 999_999_99L; // $9,999,999
        var sut = MakeSut(amount: largeAmount);
        sut.Amount.ShouldBe(largeAmount);
    }

    [Fact]
    public void Amount_WhenZero_ShouldBeAllowed()
    {
        var sut = MakeSut(amount: 0L);
        sut.Amount.ShouldBe(0L);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = MakeSut();
        var b = MakeSut();
        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentChargeId_ShouldNotBeEqual()
    {
        var a = MakeSut(chargeId: "ch_AAA");
        var b = MakeSut(chargeId: "ch_BBB");
        a.ShouldNotBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentAmount_ShouldNotBeEqual()
    {
        var a = MakeSut(amount: 1000L);
        var b = MakeSut(amount: 2000L);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentCurrency_ShouldNotBeEqual()
    {
        var a = MakeSut(currency: "USD");
        var b = MakeSut(currency: "EUR");
        a.ShouldNotBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentEmail_ShouldNotBeEqual()
    {
        var a = MakeSut(email: "a@test.com");
        var b = MakeSut(email: "b@test.com");
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = MakeSut(chargeId: "ch_Original", amount: 1000L);
        var updated = original with { ChargeId = "ch_Updated", Amount = 9999L };

        // Original is unchanged
        original.ChargeId.ShouldBe("ch_Original");
        original.Amount.ShouldBe(1000L);

        // Copy reflects changes
        updated.ChargeId.ShouldBe("ch_Updated");
        updated.Amount.ShouldBe(9999L);
        updated.Currency.ShouldBe(original.Currency);
        updated.CustomerId.ShouldBe(original.CustomerId);
        updated.ReceiptEmail.ShouldBe(original.ReceiptEmail);
        updated.Description.ShouldBe(original.Description);
    }

    [Fact]
    public void WithExpression_ChangingEmail_ShouldOnlyAffectEmail()
    {
        var original = MakeSut();
        var updated = original with { ReceiptEmail = "new@example.com" };

        original.ReceiptEmail.ShouldBe("alice@example.com");
        updated.ReceiptEmail.ShouldBe("new@example.com");
        updated.ChargeId.ShouldBe(original.ChargeId);
        updated.Currency.ShouldBe(original.Currency);
        updated.Amount.ShouldBe(original.Amount);
        updated.CustomerId.ShouldBe(original.CustomerId);
    }

    [Fact]
    public void ToString_ShouldContainKeyProperties()
    {
        var sut = MakeSut(chargeId: "ch_ToStr", currency: "EUR", amount: 750L);
        var str = sut.ToString();
        str.ShouldContain("ch_ToStr");
        str.ShouldContain("EUR");
        str.ShouldContain("750");
    }
}

// ─────────────────────────────────────────────────────────
// CardInfo
// ─────────────────────────────────────────────────────────
public class CardInfoTests
{
    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var sut = new CardInfo("John Doe", 12, 2028, "4242");

        sut.CardName.ShouldBe("John Doe");
        sut.CardExpiryMonth.ShouldBe(12);
        sut.CardExpiryYear.ShouldBe(2028);
        sut.LastDigits.ShouldBe("4242");
    }

    [Fact]
    public void ExpiryMonth_ValidBoundaries_ShouldBeStored()
    {
        var jan = new CardInfo("Jane", 1, 2030, "1111");
        var dec = new CardInfo("Jane", 12, 2030, "1111");

        jan.CardExpiryMonth.ShouldBe(1);
        dec.CardExpiryMonth.ShouldBe(12);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = new CardInfo("Alice", 6, 2027, "1234");
        var b = new CardInfo("Alice", 6, 2027, "1234");

        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentCardName_ShouldNotBeEqual()
    {
        var a = new CardInfo("Alice", 6, 2027, "1234");
        var b = new CardInfo("Bob", 6, 2027, "1234");

        a.ShouldNotBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentLastDigits_ShouldNotBeEqual()
    {
        var a = new CardInfo("Alice", 6, 2027, "1234");
        var b = new CardInfo("Alice", 6, 2027, "5678");

        a.ShouldNotBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentExpiryYear_ShouldNotBeEqual()
    {
        var a = new CardInfo("Alice", 6, 2027, "1234");
        var b = new CardInfo("Alice", 6, 2035, "1234");

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new CardInfo("Alice", 3, 2025, "0001");
        var updated = original with { CardExpiryYear = 2030, LastDigits = "9999" };

        // Original is unchanged
        original.CardExpiryYear.ShouldBe(2025);
        original.LastDigits.ShouldBe("0001");

        // Copy reflects changes
        updated.CardExpiryYear.ShouldBe(2030);
        updated.LastDigits.ShouldBe("9999");
        updated.CardName.ShouldBe(original.CardName);
        updated.CardExpiryMonth.ShouldBe(original.CardExpiryMonth);
    }

    [Fact]
    public void ToString_ShouldContainCardName()
    {
        var sut = new CardInfo("Charlie Brown", 11, 2029, "7777");
        var str = sut.ToString();
        str.ShouldContain("Charlie Brown");
        str.ShouldContain("7777");
    }
}

// ─────────────────────────────────────────────────────────
// CardNotPresentSaleRequest
// ─────────────────────────────────────────────────────────
public class CardNotPresentSaleRequestTests
{
    private static CardNotPresentSaleRequest MakeSut(
        Guid? orderId = null,
        decimal amount = 99.99m,
        string currency = "USD",
        string paymentMethod = "card",
        string? paymentMethodId = "pm_TestMethod",
        CardInfo? cardInfo = null
    ) => new(orderId ?? Guid.NewGuid(), amount, currency, paymentMethod, paymentMethodId, cardInfo);

    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var oid = Guid.NewGuid();
        var card = new CardInfo("Dave", 9, 2026, "3333");

        var sut = new CardNotPresentSaleRequest(oid, 250m, "EUR", "card", "pm_XYZ", card);

        sut.OrderId.ShouldBe(oid);
        sut.Amount.ShouldBe(250m);
        sut.Currency.ShouldBe("EUR");
        sut.PaymentMethod.ShouldBe("card");
        sut.PaymentMethodId.ShouldBe("pm_XYZ");
        sut.CardInfo.ShouldNotBeNull();
        sut.CardInfo!.CardName.ShouldBe("Dave");
    }

    [Fact]
    public void Create_WhenPaymentMethodIdIsNull_ShouldBeAllowed()
    {
        var sut = MakeSut(paymentMethodId: null);
        sut.PaymentMethodId.ShouldBeNull();
    }

    [Fact]
    public void Create_WhenCardInfoIsNull_ShouldBeAllowed()
    {
        var sut = MakeSut(cardInfo: null);
        sut.CardInfo.ShouldBeNull();
    }

    [Fact]
    public void Create_WithCardInfo_ShouldExposeCardProperties()
    {
        var card = new CardInfo("Eve", 4, 2028, "5555");
        var sut = MakeSut(cardInfo: card);

        sut.CardInfo.ShouldNotBeNull();
        sut.CardInfo!.CardExpiryYear.ShouldBe(2028);
        sut.CardInfo.LastDigits.ShouldBe("5555");
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var oid = Guid.NewGuid();
        var card = new CardInfo("Frank", 7, 2027, "1234");
        var a = new CardNotPresentSaleRequest(oid, 50m, "USD", "card", "pm_1", card);
        var b = new CardNotPresentSaleRequest(oid, 50m, "USD", "card", "pm_1", card);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentOrderId_ShouldNotBeEqual()
    {
        var a = MakeSut(orderId: Guid.NewGuid());
        var b = MakeSut(orderId: Guid.NewGuid());
        a.ShouldNotBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentAmount_ShouldNotBeEqual()
    {
        var oid = Guid.NewGuid();
        var a = MakeSut(orderId: oid, amount: 100m);
        var b = MakeSut(orderId: oid, amount: 200m);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = MakeSut(amount: 50m, currency: "USD");
        var updated = original with { Amount = 75m, Currency = "CAD" };

        original.Amount.ShouldBe(50m);
        original.Currency.ShouldBe("USD");
        updated.Amount.ShouldBe(75m);
        updated.Currency.ShouldBe("CAD");
        updated.OrderId.ShouldBe(original.OrderId);
        updated.PaymentMethod.ShouldBe(original.PaymentMethod);
    }

    [Fact]
    public void ToString_ShouldContainKeyProperties()
    {
        var oid = Guid.NewGuid();
        var sut = new CardNotPresentSaleRequest(oid, 123.45m, "GBP", "card", null, null);
        var str = sut.ToString();
        str.ShouldContain("GBP");
        str.ShouldContain("card");
    }
}
