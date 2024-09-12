using ErrorOr;
using Shopizy.Application.Common.models;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Payments.Commands.CreatePayment;

[Authorize(Permissions = Permissions.Order.Get, Policies = Policy.SelfOrAdmin)]
public record CreatePaymentCommand(Guid UserId, Guid OrderId)
    : IAuthorizeableRequest<ErrorOr<ChargeResource>>;
