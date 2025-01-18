namespace Shopizy.Contracts.Order;

public record OrdersCriteria(
    Guid? CustomerId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10,
    OrderStatus? Status = null
);

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6,
}
