using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Products.Events;

public record ProductCreatedDomainEvent(Product Product) : IDomainEvent;
