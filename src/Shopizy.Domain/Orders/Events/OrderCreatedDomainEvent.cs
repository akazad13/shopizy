using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Orders.Events;

public record OrderCreatedDomainEvent(Order Order) : IDomainEvent;
