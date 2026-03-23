using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetOrdersAsync(
        UserId? customerId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatus? status,
        int pageNumber,
        int pageSize,
        OrderType orderType = OrderType.Ascending
    );
    Task<int> GetTotalOrdersCountAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<decimal> GetRevenueByPeriodAsync(DateTime start, DateTime end);
    Task<IReadOnlyList<TopProductDto>> GetTopProductsByRevenueAsync(int count);
    Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersBySpendAsync(int count);
    Task<IReadOnlyList<Order>> GetOrdersByIdsAsync(IList<OrderId> ids);
    Task<Order?> GetOrderByIdAsync(OrderId id);
    Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task AddAsync(Order order);
    void Update(Order order);
}

