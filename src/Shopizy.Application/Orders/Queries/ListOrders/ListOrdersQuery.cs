using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Queries.ListOrders;

[Authorize(Permissions = Permission.Order.Get, Policies = Policy.SelfOrAdmin)]
public record ListOrdersQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<List<Order>>>;
