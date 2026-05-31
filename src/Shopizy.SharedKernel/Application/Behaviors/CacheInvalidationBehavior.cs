using ErrorOr;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.SharedKernel.Application.Behaviors;

/// <summary>
/// After a command completes successfully (including its UnitOfWork commit),
/// remove any cache entries the command declares as stale.
/// Wrapped after <see cref="UnitOfWorkCommandHandlerDecorator{TCommand, TResponse}"/>
/// so cache eviction only happens once the write is durable.
/// </summary>
/// <param name="innerHandler"></param>
/// <param name="cacheHelper"></param>
public class CacheInvalidationCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    ICacheHelper cacheHelper
) : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await innerHandler.Handle(command, cancellationToken);

        if (response.IsError || command is not IInvalidateCache invalidator)
        {
            return response;
        }

        foreach (var key in invalidator.CacheKeysToInvalidate)
        {
            if (!string.IsNullOrEmpty(key))
            {
                await cacheHelper.RemoveAsync(key);
            }
        }

        return response;
    }
}
