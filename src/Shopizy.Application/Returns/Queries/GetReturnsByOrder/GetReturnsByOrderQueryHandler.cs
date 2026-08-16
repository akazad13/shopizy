using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Queries.GetReturnsByOrder;

public class GetReturnsByOrderQueryHandler(IReturnRequestRepository returnRequestRepository)
    : IQueryHandler<GetReturnsByOrderQuery, ErrorOr<IReadOnlyList<ReturnRequest>>>
{
    public async Task<ErrorOr<IReadOnlyList<ReturnRequest>>> Handle(
        GetReturnsByOrderQuery request,
        CancellationToken cancellationToken
    )
    {
        var orderId = OrderId.Create(request.OrderId);
        var returns = await returnRequestRepository.GetByOrderIdAsync(orderId, cancellationToken);
        return returns.ToList().AsReadOnly();
    }
}
