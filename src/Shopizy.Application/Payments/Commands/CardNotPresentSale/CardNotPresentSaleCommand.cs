using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Payments.Commands.CardNotPresentSale;

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
