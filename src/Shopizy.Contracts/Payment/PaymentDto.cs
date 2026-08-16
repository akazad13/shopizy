namespace Shopizy.Contracts.Payment;

public record PaymentDto(
    Guid PaymentId,
    Guid OrderId,
    Guid UserId,
    string PaymentMethod,
    string PaymentMethodId,
    string TransactionId,
    string PaymentStatus,
    decimal TotalAmount,
    string Currency,
    string BillingStreet,
    string BillingCity,
    string BillingState,
    string BillingCountry,
    string BillingZipCode,
    DateTime CreatedOn,
    DateTime? ModifiedOn
);
