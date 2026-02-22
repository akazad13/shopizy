namespace Shopizy.SharedKernel.Domain.Models;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IReadOnlyList<IDomainEvent> PopDomainEvents();
}
