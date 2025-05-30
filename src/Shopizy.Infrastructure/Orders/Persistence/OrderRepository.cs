using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Infrastructure.Orders.Specifications;

namespace Shopizy.Infrastructure.Orders.Persistence;

public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<List<Order>> GetOrdersAsync(
        UserId? customerId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatus? status,
        int pageNumber,
        int pageSize,
        OrderType orderType = OrderType.Ascending
    )
    {
        return ApplySpec(
                new OrdersByCriteriaSpec(
                    customerId,
                    startDate,
                    endDate,
                    status,
                    pageNumber,
                    pageSize,
                    orderType
                )
            )
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Order?> GetOrderByIdAsync(OrderId id)
    {
        return ApplySpec(new OrderByIdSpec(id)).FirstOrDefaultAsync();
    }

    public IQueryable<Order> GetOrdersByUserId(UserId userId)
    {
        return _dbContext.Orders.Where(o => o.UserId == userId);
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

    private IQueryable<Order> ApplySpec(Specification<Order> spec)
    {
        return SpecificationEvaluator.GetQuery(_dbContext.Orders, spec);
    }
}
