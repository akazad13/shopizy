using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Payments.Events;

public record PaymentSucceededDomainEvent(OrderId OrderId, string CustomerId) : IDomainEvent;
