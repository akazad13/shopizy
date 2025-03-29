using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Orders.Enums;

namespace Shopizy.Application.Orders.Queries.GetOrders;

[Authorize(Permissions = Permissions.Order.Get)]
public record GetOrdersQuery(
    Guid UserId,
    Guid? CustomerId,
    DateTime? StartDate,
    DateTime? EndDate,
    OrderStatus? Status,
    int PageNumber = 1,
    int PageSize = 10
) : IAuthorizeableRequest<ErrorOr<List<OrderDto>?>>;
