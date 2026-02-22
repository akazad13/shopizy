using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.Events;

public record OrderCancelledDomainEvent(Order Order) : IDomainEvent;
