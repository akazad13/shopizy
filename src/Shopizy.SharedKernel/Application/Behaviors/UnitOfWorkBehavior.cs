using ErrorOr;
using MediatR;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;

namespace Shopizy.SharedKernel.Application.Behaviors;

public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork _unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
#pragma warning disable CA2016
        var response = await next();
#pragma warning restore CA2016

        if (response.IsError)
        {
            return response;
        }

        // Only commit if the request is not a Query (conventional check)
        // Or we can rely on IUnitOfWork being scoped and only saving changes if entities are tracked.
        if (request.GetType().Name.EndsWith("Command"))
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
