using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Returns;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Queries.GetPendingReturns;

public class GetPendingReturnsQueryHandler(IReturnRequestRepository returnRequestRepository)
    : IQueryHandler<GetPendingReturnsQuery, ErrorOr<IReadOnlyList<ReturnRequest>>>
{
    public async Task<ErrorOr<IReadOnlyList<ReturnRequest>>> Handle(
        GetPendingReturnsQuery request,
        CancellationToken cancellationToken
    )
    {
        var returns = await returnRequestRepository.GetPendingReturnsAsync(cancellationToken);
        return returns.ToList().AsReadOnly();
    }
}
