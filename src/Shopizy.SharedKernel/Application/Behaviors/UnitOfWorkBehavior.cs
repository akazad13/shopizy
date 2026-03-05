using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;

namespace Shopizy.SharedKernel.Application.Behaviors;

public class UnitOfWorkCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    IUnitOfWork unitOfWork)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : IErrorOr
{
    private readonly ICommandHandler<TCommand, TResponse> _innerHandler = innerHandler;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var response = await _innerHandler.Handle(command, cancellationToken);

        if (response.IsError)
        {
            return response;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
