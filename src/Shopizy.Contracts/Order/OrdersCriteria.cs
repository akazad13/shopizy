namespace Shopizy.Contracts.Order;

/// <summary>
/// Represents criteria for searching and filtering orders.
/// </summary>
/// <param name="CustomerId">Filter by customer identifier.</param>
/// <param name="StartDate">Filter by start date.</param>
/// <param name="EndDate">Filter by end date.</param>
/// <param name="PageNumber">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="Status">Filter by order status.</param>
public record OrdersCriteria(
    Guid? CustomerId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10,
    OrderStatus? Status = null
);

/// <summary>
/// Defines the possible statuses of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order is pending.</summary>
    Pending = 1,
    /// <summary>Order is being processed.</summary>
    Processing = 2,
    /// <summary>Order has been shipped.</summary>
    Shipped = 3,
    /// <summary>Order has been delivered.</summary>
    Delivered = 4,
    /// <summary>Order has been cancelled.</summary>
    Cancelled = 5,
    /// <summary>Order has been refunded.</summary>
    Refunded = 6,
}
