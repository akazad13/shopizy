using ErrorOr;
using Shopizy.Application.Common.models;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Payments.Commands.CreatePaymentSession;

[Authorize(Permissions = Permissions.Order.Get, Policies = Policy.SelfOrAdmin)]
public record CreatePaymentSessionCommand(
    Guid UserId,
    Guid OrderId,
    string PaymentType,
    string SuccessUrl,
    string CancelUrl
) : IAuthorizeableRequest<ErrorOr<CheckoutSession>>;
