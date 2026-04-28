using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.Events;

public record PaymentCompletedDomainEvent(OrderId OrderId, UserId UserId, string CustomerId)
    : IDomainEvent;
