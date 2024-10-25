using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Queries.GetOrder;

[Authorize(Permissions = Permissions.Order.Get, Policies = Policy.SelfOrAdmin)]
public record GetOrderQuery(Guid UserId, Guid OrderId) : IAuthorizeableRequest<IResult<Order>>;
