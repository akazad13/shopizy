namespace Shopizy.Contracts.Payment;

public record PaymentResponse(
    string ChargeId,
    string Currency,
    long Amount,
    string CustomerId,
    string ReceiptEmail,
    string Description
);
