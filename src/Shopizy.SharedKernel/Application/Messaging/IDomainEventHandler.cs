using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.SharedKernel.Application.Messaging;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);
}
