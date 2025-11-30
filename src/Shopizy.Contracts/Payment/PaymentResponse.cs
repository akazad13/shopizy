namespace Shopizy.Contracts.Payment;

/// <summary>
/// Represents a payment response.
/// </summary>
/// <param name="ChargeId">The unique identifier of the charge.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="Amount">The amount charged.</param>
/// <param name="CustomerId">The customer identifier.</param>
/// <param name="ReceiptEmail">The email address for the receipt.</param>
/// <param name="Description">The description of the charge.</param>
public record PaymentResponse(
    string ChargeId,
    string Currency,
    long Amount,
    string CustomerId,
    string ReceiptEmail,
    string Description
);
