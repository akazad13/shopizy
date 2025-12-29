using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Orders.Events;

public record OrderCancelledDomainEvent(Order Order) : IDomainEvent;
