using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Queries.ListOrders;

[Authorize(Permissions = Permissions.Order.Get, Policies = Policy.Admin)]
public record ListOrdersQuery() : IAuthorizeableRequest<ErrorOr<List<Order>?>>;
