using Microsoft.Extensions.DependencyInjection;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Messaging;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        Type commandType = command.GetType();
        Type handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

        object? handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"No handler registered for {commandType.Name}");

        await ((Task)handlerType.GetMethod("Handle")!.Invoke(handler, [command, cancellationToken])!).ConfigureAwait(false);
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        Type commandType = command.GetType();
        Type handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));

        object? handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"No handler registered for {commandType.Name}");

        return await ((Task<TResponse>)handlerType.GetMethod("Handle")!.Invoke(handler, [command, cancellationToken])!).ConfigureAwait(false);
    }

    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        Type queryType = query.GetType();
        Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

        object? handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"No handler registered for {queryType.Name}");

        return await ((Task<TResponse>)handlerType.GetMethod("Handle")!.Invoke(handler, [query, cancellationToken])!).ConfigureAwait(false);
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
    {
        Type eventType = domainEvent.GetType();
        Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        // Get all registered handlers for this event type
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = handlers.Select(handler =>
            (Task)handlerType.GetMethod("Handle")!.Invoke(handler, [domainEvent, cancellationToken])!);

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
