using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Messaging;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    // Cache keyed by (messageType, responseType) -> (handlerType, MethodInfo).
    // responseType is typeof(void) for fire-and-forget commands.
    private static readonly ConcurrentDictionary<
        (Type MessageType, Type ResponseType),
        (Type HandlerType, MethodInfo Method)
    > _cache = new();

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        Type commandType = command.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (commandType, typeof(void)),
            static k =>
            {
                var ht = typeof(ICommandHandler<>).MakeGenericType(k.MessageType);
                return (ht, ht.GetMethod("Handle")!);
            }
        );

        object? handler =
            _serviceProvider.GetService(handlerType)
            ?? throw NoHandlerRegistered(
                commandType,
                responseType: null,
                handlerInterface: typeof(ICommandHandler<>)
            );

        await ((Task)method.Invoke(handler, [command, cancellationToken])!).ConfigureAwait(false);
    }

    public async Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default
    )
    {
        Type commandType = command.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (commandType, typeof(TResponse)),
            static k =>
            {
                var ht = typeof(ICommandHandler<,>).MakeGenericType(k.MessageType, k.ResponseType);
                return (ht, ht.GetMethod("Handle")!);
            }
        );

        object? handler =
            _serviceProvider.GetService(handlerType)
            ?? throw NoHandlerRegistered(
                commandType,
                typeof(TResponse),
                handlerInterface: typeof(ICommandHandler<,>)
            );

        return await (
            (Task<TResponse>)method.Invoke(handler, [command, cancellationToken])!
        ).ConfigureAwait(false);
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default
    )
    {
        Type queryType = query.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (queryType, typeof(TResponse)),
            static k =>
            {
                var ht = typeof(IQueryHandler<,>).MakeGenericType(k.MessageType, k.ResponseType);
                return (ht, ht.GetMethod("Handle")!);
            }
        );

        object? handler =
            _serviceProvider.GetService(handlerType)
            ?? throw NoHandlerRegistered(
                queryType,
                typeof(TResponse),
                handlerInterface: typeof(IQueryHandler<,>)
            );

        return await (
            (Task<TResponse>)method.Invoke(handler, [query, cancellationToken])!
        ).ConfigureAwait(false);
    }

    private static InvalidOperationException NoHandlerRegistered(
        Type messageType,
        Type? responseType,
        Type handlerInterface
    )
    {
        var signature = responseType is null
            ? $"{handlerInterface.Name.TrimEnd('`', '1')}<{messageType.FullName}>"
            : $"{handlerInterface.Name.TrimEnd('`', '2')}<{messageType.FullName}, {responseType.FullName}>";
        return new InvalidOperationException(
            $"No handler registered for {messageType.FullName}. Expected DI registration: {signature}. "
                + "Verify the handler class is in an assembly scanned by Shopizy.Application.DependencyInjectionRegister and implements the corresponding handler interface."
        );
    }

    public async Task PublishAsync<TEvent>(
        TEvent domainEvent,
        CancellationToken cancellationToken = default
    )
        where TEvent : IDomainEvent
    {
        Type eventType = domainEvent.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (eventType, typeof(void)),
            static k =>
            {
                var ht = typeof(IDomainEventHandler<>).MakeGenericType(k.MessageType);
                return (ht, ht.GetMethod("Handle")!);
            }
        );

        // Get all registered handlers for this event type.
        // Run sequentially to avoid concurrent DbContext access (DbContext is not thread-safe).
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            await ((Task)method.Invoke(handler, [domainEvent, cancellationToken])!).ConfigureAwait(
                false
            );
        }
    }
}
