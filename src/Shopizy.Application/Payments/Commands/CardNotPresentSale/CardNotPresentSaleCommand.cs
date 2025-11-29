using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Payments.Commands.CardNotPresentSale;

/// <summary>
/// Represents a command to process a card-not-present payment sale.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="OrderId">The order's unique identifier.</param>
/// <param name="Amount">The payment amount.</param>
/// <param name="Currency">The payment currency.</param>
/// <param name="PaymentMethod">The payment method type.</param>
/// <param name="PaymentMethodId">The payment method identifier.</param>
/// <param name="CardName">The cardholder's name.</param>
/// <param name="CardExpiryMonth">The card expiry month.</param>
/// <param name="CardExpiryYear">The card expiry year.</param>
/// <param name="LastDigits">The last digits of the card.</param>
[Authorize(Permissions = Permissions.Order.Create)]
public record CardNotPresentSaleCommand(
    Guid UserId,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string PaymentMethodId,
    string CardName,
    int CardExpiryMonth,
    int CardExpiryYear,
    string LastDigits
) : IAuthorizeableRequest<ErrorOr<Success>>;
