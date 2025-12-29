using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Products.Events;

public record ProductCreatedDomainEvent(Product Product) : IDomainEvent;
