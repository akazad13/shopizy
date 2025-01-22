using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Orders.Queries.GetOrders;

public record OrderDto(
    OrderId Id,
    UserId UserId,
    Price Total,
    OrderStatus OrderStatus,
    DateTime CreatedOn
);
