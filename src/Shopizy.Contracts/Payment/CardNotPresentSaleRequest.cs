namespace Shopizy.Contracts.Payment;

public record CardNotPresentSaleRequest(
    Guid OrderId,
    double Amount,
    string Currency,
    string PaymentMethod,
    string PaymentMethodId,
    string CardName,
    string CardExpiryMonth,
    string CardExpiryYear,
    string LastDigits
);
