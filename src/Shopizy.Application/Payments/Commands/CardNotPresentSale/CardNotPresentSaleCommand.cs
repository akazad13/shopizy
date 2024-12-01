using ErrorOr;
using Shopizy.Application.Common.models;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Payments.Commands.CardNotPresentSale;

[Authorize(Permissions = Permissions.Order.Get, Policies = Policy.SelfOrAdmin)]
public record CardNotPresentSaleCommand(
    Guid UserId,
    Guid OrderId,
    double Amount,
    string Currency,
    string PaymentMethod,
    string CardName,
    string CardExpiryMonth,
    string CardExpiryYear,
    string LastDigits
) : IAuthorizeableRequest<ErrorOr<Success>>;
