using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Queries.GetOrder;

[Authorize(Permissions = Permissions.Order.Get)]
public record GetOrderQuery(Guid UserId, Guid OrderId) : IAuthorizeableRequest<ErrorOr<Order>>;
