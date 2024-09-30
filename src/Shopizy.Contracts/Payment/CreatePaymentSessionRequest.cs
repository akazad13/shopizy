namespace Shopizy.Contracts.Payment;

public record CreatePaymentSessionRequest(
    Guid OrderId,
    string PaymentType,
    string SuccessUrl,
    string CancelUrl
);
