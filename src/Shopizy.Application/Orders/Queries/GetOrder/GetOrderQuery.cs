using ErrorOr;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Queries.GetOrder;

/// <summary>
/// Represents a query to retrieve a specific order by its ID.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="OrderId">The order's unique identifier.</param>
public record GetOrderQuery(Guid UserId, Guid OrderId) : MediatR.IRequest<ErrorOr<Order>>;
