using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.Enums;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Returns.Persistence;

public class ReturnRequestRepository(AppDbContext dbContext) : IReturnRequestRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(ReturnRequest returnRequest, CancellationToken cancellationToken) =>
        await _dbContext.ReturnRequests.AddAsync(returnRequest, cancellationToken);

    public Task<ReturnRequest?> GetByIdAsync(
        ReturnRequestId id,
        CancellationToken cancellationToken
    ) =>
        _dbContext
            .ReturnRequests.Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ReturnRequest>> GetByOrderIdAsync(
        OrderId orderId,
        CancellationToken cancellationToken
    ) =>
        await _dbContext
            .ReturnRequests.Include(r => r.Items)
            .Where(r => r.OrderId == orderId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ReturnRequest>> GetPendingReturnsAsync(
        CancellationToken cancellationToken
    ) =>
        await _dbContext
            .ReturnRequests.Include(r => r.Items)
            .Where(r => r.Status == ReturnStatus.Pending)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public void Update(ReturnRequest returnRequest) =>
        _dbContext.ReturnRequests.Update(returnRequest);
}
