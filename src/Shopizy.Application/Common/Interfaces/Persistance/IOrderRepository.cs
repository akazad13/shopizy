using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;

namespace shopizy.Application.Common.Interfaces.Persistance;

public interface IOrderRepository
{
    Task<List<Order>> GetOrdersAsync();
    Task<Order?> GetOrderByIdAsync(OrderId id);
    Task AddAsync(Order order);
    void Update(Order order);
    Task<int> Commit(CancellationToken cancellationToken);
}
