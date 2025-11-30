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
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A queryable of orders for the user.</returns>
    public IQueryable<Order> GetOrdersByUserId(UserId userId)
    {
        return _dbContext.Orders.Where(o => o.UserId == userId);
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
    /// Commits all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
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
