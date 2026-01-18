using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IOrderRepository
{
    Task<List<Order>> GetOrdersAsync(
        UserId? customerId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatus? status,
        int pageNumber,
        int pageSize,
        OrderType orderType = OrderType.Ascending
    );
    Task<Order?> GetOrderByIdAsync(OrderId id);
    IQueryable<Order> GetOrdersByUserId(UserId userId);
    Task AddAsync(Order order);
    void Update(Order order);
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
