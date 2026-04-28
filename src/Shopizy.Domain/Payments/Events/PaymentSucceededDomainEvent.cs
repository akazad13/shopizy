using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Payments.Events;

public record PaymentSucceededDomainEvent(OrderId OrderId, string CustomerId) : IDomainEvent;
