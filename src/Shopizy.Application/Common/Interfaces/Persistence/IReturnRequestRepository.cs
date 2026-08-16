using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IReturnRequestRepository
{
    Task AddAsync(ReturnRequest returnRequest, CancellationToken cancellationToken);
    Task<ReturnRequest?> GetByIdAsync(ReturnRequestId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReturnRequest>> GetByOrderIdAsync(
        OrderId orderId,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ReturnRequest>> GetPendingReturnsAsync(CancellationToken cancellationToken);
    void Update(ReturnRequest returnRequest);
}
