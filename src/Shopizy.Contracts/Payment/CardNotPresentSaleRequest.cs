namespace Shopizy.Contracts.Payment;

public record CardNotPresentSaleRequest(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string? PaymentMethodId,
    CardInfo? CardInfo
);

public record CardInfo(string CardName, int CardExpiryMonth, int CardExpiryYear, string LastDigits);
