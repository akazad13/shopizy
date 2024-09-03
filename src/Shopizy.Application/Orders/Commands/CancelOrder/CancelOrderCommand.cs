using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Orders.Commands.CancelOrder;

[Authorize(Permissions = Permissions.Order.Modify, Policies = Policy.SelfOrAdmin)]
public record CancelOrderCommand(Guid UserId, Guid OrderId, string Reason)
    : IAuthorizeableRequest<ErrorOr<Success>>;
