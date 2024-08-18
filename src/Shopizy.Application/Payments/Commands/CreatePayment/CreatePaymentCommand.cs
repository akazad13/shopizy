using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.models;

namespace Shopizy.Application.Payments.Commands.CreatePayment;

[Authorize(Permissions = Permission.Order.Get, Policies = Policy.SelfOrAdmin)]
public record CreatePaymentCommand(Guid UserId, Guid OrderId)
    : IAuthorizeableRequest<ErrorOr<CheckoutSession>>;
