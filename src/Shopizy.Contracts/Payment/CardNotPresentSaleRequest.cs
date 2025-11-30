namespace Shopizy.Contracts.Payment;

/// <summary>
/// Represents a request for a card-not-present sale (e.g., online payment).
/// </summary>
/// <param name="OrderId">The unique identifier of the order.</param>
/// <param name="Amount">The amount to charge.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="PaymentMethod">The payment method (e.g., "card").</param>
/// <param name="PaymentMethodId">The payment method identifier from the provider.</param>
/// <param name="CardInfo">The card information (optional).</param>
public record CardNotPresentSaleRequest(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string? PaymentMethodId,
    CardInfo? CardInfo
);

/// <summary>
/// Represents credit card information.
/// </summary>
/// <param name="CardName">The name on the card.</param>
/// <param name="CardExpiryMonth">The card expiry month.</param>
/// <param name="CardExpiryYear">The card expiry year.</param>
/// <param name="LastDigits">The last 4 digits of the card number.</param>
public record CardInfo(string CardName, int CardExpiryMonth, int CardExpiryYear, string LastDigits);
