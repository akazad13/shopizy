using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Queries.GetReturnById;

public class GetReturnByIdQueryHandler(IReturnRequestRepository returnRequestRepository)
    : IQueryHandler<GetReturnByIdQuery, ErrorOr<ReturnRequest>>
{
    public async Task<ErrorOr<ReturnRequest>> Handle(
        GetReturnByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var returnId = ReturnRequestId.Create(request.ReturnId);
        var returnRequest = await returnRequestRepository.GetByIdAsync(returnId, cancellationToken);

        if (returnRequest is null)
        {
            return (Error)CustomErrors.ReturnRequest.ReturnNotFound;
        }

        return returnRequest;
    }
}
