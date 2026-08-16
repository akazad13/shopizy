using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Returns.Commands.RejectReturn;

public class RejectReturnCommandHandler(
    IReturnRequestRepository returnRequestRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<RejectReturnCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RejectReturnCommand request,
        CancellationToken cancellationToken
    )
    {
        var returnId = ReturnRequestId.Create(request.ReturnRequestId);
        var returnRequest = await returnRequestRepository.GetByIdAsync(returnId, cancellationToken);

        if (returnRequest is null)
        {
            return (Error)CustomErrors.ReturnRequest.ReturnNotFound;
        }

        var rejectResult = returnRequest.Reject(request.AdminNote);
        if (rejectResult.IsError)
        {
            return rejectResult.Error.ToError();
        }

        returnRequestRepository.Update(returnRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
