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
    private static readonly ConcurrentDictionary<(Type MessageType, Type ResponseType), (Type HandlerType, MethodInfo Method)> _cache = new();

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        Type commandType = command.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (commandType, typeof(void)),
            static k =>
            {
                var ht = typeof(ICommandHandler<>).MakeGenericType(k.MessageType);
                return (ht, ht.GetMethod("Handle")!);
            });

        object? handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"No handler registered for {commandType.Name}");

        await ((Task)method.Invoke(handler, [command, cancellationToken])!).ConfigureAwait(false);
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        Type commandType = command.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (commandType, typeof(TResponse)),
            static k =>
            {
                var ht = typeof(ICommandHandler<,>).MakeGenericType(k.MessageType, k.ResponseType);
                return (ht, ht.GetMethod("Handle")!);
            });

        object? handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"No handler registered for {commandType.Name}");

        return await ((Task<TResponse>)method.Invoke(handler, [command, cancellationToken])!).ConfigureAwait(false);
    }

    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        Type queryType = query.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (queryType, typeof(TResponse)),
            static k =>
            {
                var ht = typeof(IQueryHandler<,>).MakeGenericType(k.MessageType, k.ResponseType);
                return (ht, ht.GetMethod("Handle")!);
            });

        object? handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"No handler registered for {queryType.Name}");

        return await ((Task<TResponse>)method.Invoke(handler, [query, cancellationToken])!).ConfigureAwait(false);
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
    {
        Type eventType = domainEvent.GetType();
        var (handlerType, method) = _cache.GetOrAdd(
            (eventType, typeof(void)),
            static k =>
            {
                var ht = typeof(IDomainEventHandler<>).MakeGenericType(k.MessageType);
                return (ht, ht.GetMethod("Handle")!);
            });

        // Get all registered handlers for this event type
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = handlers.Select(handler =>
            (Task)method.Invoke(handler, [domainEvent, cancellationToken])!);

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
