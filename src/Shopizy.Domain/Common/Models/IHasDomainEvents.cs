namespace Shopizy.Domain.Common.Models;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    List<IDomainEvent> PopDomainEvents();
}
