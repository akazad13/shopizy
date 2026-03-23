using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Admin.Queries.GetSalesReport;
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

/// <summary>
/// Repository for managing order data persistence.
/// </summary>
public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves a paginated list of orders based on search criteria.
    /// </summary>
    /// <param name="customerId">Optional customer ID filter.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="status">Optional order status filter.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="orderType">The sort order (ascending or descending).</param>
    /// <returns>A list of orders matching the criteria.</returns>
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
        return await ApplySpec(
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

    public Task<int> GetTotalOrdersCountAsync()
    {
        return _dbContext.Orders.CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        var orders = await _dbContext.Orders.Include(o => o.OrderItems).ToListAsync();
        return orders.Sum(o => o.GetTotal().Amount);
    }

    public async Task<decimal> GetRevenueByPeriodAsync(DateTime start, DateTime end)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CreatedOn >= start && o.CreatedOn <= end)
            .ToListAsync();
        return orders.Sum(o => o.GetTotal().Amount);
    }

    public async Task<IReadOnlyList<TopProductDto>> GetTopProductsByRevenueAsync(int count)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .ToListAsync();

        var topProducts = orders
            .SelectMany(o => o.OrderItems)
            .GroupBy(item => item.Name)
            .Select(g => new TopProductDto(
                g.Key,
                g.Sum(item => item.Quantity),
                g.Sum(item => item.UnitPrice.Amount * item.Quantity)
            ))
            .OrderByDescending(p => p.Revenue)
            .Take(count)
            .ToList();

        return topProducts;
    }

    public async Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersBySpendAsync(int count)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .ToListAsync();

        var customerSpend = orders
            .GroupBy(o => o.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalSpend = g.Sum(o => o.GetTotal().Amount)
            })
            .OrderByDescending(x => x.TotalSpend)
            .Take(count)
            .ToList();

        var userIds = customerSpend.Select(x => x.UserId).ToList();
        var users = await _dbContext.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        var topCustomers = customerSpend
            .Join(users, cs => cs.UserId, u => u.Id, (cs, u) => new TopCustomerDto(
                u.Id.Value,
                u.FirstName,
                u.LastName,
                cs.TotalSpend
            ))
            .ToList();

        return topCustomers;
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByIdsAsync(IList<OrderId> ids)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderItems)
            .Where(o => ids.Contains(o.Id))
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <returns>The order if found; otherwise, null.</returns>
    public Task<Order?> GetOrderByIdAsync(OrderId id)
    {
        return ApplySpec(new OrderByIdSpec(id)).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all orders for a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of orders for the user.</returns>
    public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Where(o => o.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new order to the database.
    /// </summary>
    /// <param name="order">The order to add.</param>
    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }

    /// <summary>
    /// Updates an existing order in the database.
    /// </summary>
    /// <param name="order">The order to update.</param>
    public void Update(Order order)
    {
        _dbContext.Update(order);
    }

    /// <summary>
    /// Applies a specification to the order query.
    /// </summary>
    /// <param name="spec">The specification to apply.</param>
    /// <returns>A queryable of orders.</returns>
    private IQueryable<Order> ApplySpec(Specification<Order> spec)
    {
        return SpecificationEvaluator.GetQuery(_dbContext.Orders, spec);
    }
}
