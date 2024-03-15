using Microsoft.EntityFrameworkCore;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Orders.Persistence;

public class OrderRepository(AppDbContext _dbContext) : IOrderRepository
{
    public Task<List<Order>> GetOrdersAsync()
    {
        return _dbContext.Orders.AsNoTracking().ToListAsync();
    }
    public Task<Order?> GetOrderByIdAsync(OrderId id)
    {
        return _dbContext.Orders.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }
    public void Update(Order order)
    {
        _dbContext.Update(order);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}