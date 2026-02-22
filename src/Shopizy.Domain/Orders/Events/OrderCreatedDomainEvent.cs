using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.Events;

public record OrderCreatedDomainEvent(Order Order) : IDomainEvent;
