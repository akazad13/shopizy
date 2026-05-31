using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Orders.Persistence;

/// <summary>
/// Repository for managing order data persistence.
/// </summary>
/// <param name="dbContext"></param>
public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves a paginated list of orders based on search criteria.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="status"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="orderType"></param>
    public async Task<IReadOnlyList<Order>> GetOrdersAsync(
        UserId? customerId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatus? status,
        int pageNumber,
        int pageSize,
        OrderType orderType = OrderType.Ascending
    )
    {
        var query = _dbContext.Orders.AsQueryable();

        if (customerId is not null)
            query = query.Where(o => o.UserId == customerId);
        if (startDate is not null)
            query = query.Where(o => o.CreatedOn >= startDate);
        if (endDate is not null)
            query = query.Where(o => o.CreatedOn <= endDate);
        if (status is not null)
            query = query.Where(o => o.OrderStatus == status);

        var ordered =
            orderType == OrderType.Descending
                ? query.OrderByDescending(o => o.CreatedOn)
                : query.OrderBy(o => o.CreatedOn);

        return await ordered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<int> GetTotalOrdersCountAsync() => _dbContext.Orders.CountAsync();

    public Task<int> GetOrdersCountAsync(
        UserId? customerId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatus? status
    )
    {
        var query = _dbContext.Orders.AsQueryable();

        if (customerId is not null)
            query = query.Where(o => o.UserId == customerId);
        if (startDate is not null)
            query = query.Where(o => o.CreatedOn >= startDate);
        if (endDate is not null)
            query = query.Where(o => o.CreatedOn <= endDate);
        if (status is not null)
            query = query.Where(o => o.OrderStatus == status);

        return query.CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync() =>
        await _dbContext
            .Orders.AsSingleQuery()
            .SelectMany(o => o.OrderItems)
            .SumAsync(i => i.UnitPrice.Amount * i.Quantity);

    public Task<int> GetOrdersCountByPeriodAsync(DateTime start, DateTime end) =>
        _dbContext.Orders.Where(o => o.CreatedOn >= start && o.CreatedOn <= end).CountAsync();

    public async Task<decimal> GetRevenueByPeriodAsync(DateTime start, DateTime end) =>
        await _dbContext
            .Orders.AsSingleQuery()
            .Where(o => o.CreatedOn >= start && o.CreatedOn <= end)
            .SelectMany(o => o.OrderItems)
            .SumAsync(i => i.UnitPrice.Amount * i.Quantity);

    public async Task<IReadOnlyList<TopProductDto>> GetTopProductsByRevenueAsync(int count) =>
        await _dbContext
            .Orders.AsSingleQuery()
            .SelectMany(o => o.OrderItems)
            .GroupBy(item => item.Name)
            .Select(g => new TopProductDto(
                g.Key,
                g.Sum(item => item.Quantity),
                g.Sum(item => item.UnitPrice.Amount * item.Quantity)
            ))
            .OrderByDescending(p => p.Revenue)
            .Take(count)
            .ToListAsync();

    public async Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersBySpendAsync(int count)
    {
        var customerSpend = await _dbContext
            .Orders.AsSingleQuery()
            .GroupBy(o => o.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalSpend = g.Sum(o =>
                    o.OrderItems.Sum(i => i.UnitPrice.Amount * i.Quantity) + o.DeliveryCharge.Amount
                ),
            })
            .OrderByDescending(x => x.TotalSpend)
            .Take(count)
            .ToListAsync();

        var userIds = customerSpend.Select(x => x.UserId).ToList();
        var users = await _dbContext
            .Users.Where(u => userIds.Contains(u.Id))
            .AsNoTracking()
            .ToListAsync();

        return customerSpend
            .Join(
                users,
                cs => cs.UserId,
                u => u.Id,
                (cs, u) => new TopCustomerDto(u.Id.Value, u.FirstName, u.LastName, cs.TotalSpend)
            )
            .ToList();
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByIdsAsync(IList<OrderId> ids) =>
        await _dbContext
            .Orders.Include(o => o.OrderItems)
            .Where(o => ids.Contains(o.Id))
            .ToListAsync();

    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    public Task<Order?> GetOrderByIdAsync(OrderId id) =>
        _dbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);

    /// <summary>
    /// Retrieves all orders for a specific user asynchronously.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    ) =>
        await _dbContext
            .Orders.Where(o => o.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Adds a new order to the database.
    /// </summary>
    /// <param name="order"></param>
    public async Task AddAsync(Order order) => await _dbContext.Orders.AddAsync(order);

    /// <summary>
    /// Updates an existing order in the database.
    /// </summary>
    /// <param name="order"></param>
    public void Update(Order order) => _dbContext.Update(order);
}
