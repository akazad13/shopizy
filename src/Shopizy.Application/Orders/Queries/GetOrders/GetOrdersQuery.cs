using ErrorOr;
using Shopizy.Domain.Orders.Enums;

namespace Shopizy.Application.Orders.Queries.GetOrders;

/// <summary>
/// Represents a query to retrieve a paginated list of orders with optional filtering.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CustomerId">Optional customer ID filter.</param>
/// <param name="StartDate">Optional start date filter.</param>
/// <param name="EndDate">Optional end date filter.</param>
/// <param name="Status">Optional order status filter.</param>
/// <param name="PageNumber">The page number (default: 1).</param>
/// <param name="PageSize">The page size (default: 10).</param>
public record GetOrdersQuery(
    Guid UserId,
    Guid? CustomerId,
    DateTime? StartDate,
    DateTime? EndDate,
    OrderStatus? Status,
    int PageNumber = 1,
    int PageSize = 10
) : MediatR.IRequest<ErrorOr<List<OrderDto>?>>;
