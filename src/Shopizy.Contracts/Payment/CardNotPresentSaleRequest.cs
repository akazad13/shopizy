namespace Shopizy.Contracts.Payment;

public record CardNotPresentSaleRequest(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string? PaymentMethodId,
    CardInfo? CardInfo
);

public record CardInfo(
    string CardName,
    string CardExpiryMonth,
    string CardExpiryYear,
    string LastDigits
);
