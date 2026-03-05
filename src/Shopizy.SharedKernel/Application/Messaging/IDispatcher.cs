using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.SharedKernel.Application.Messaging;

public interface IDispatcher
{
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;
}
