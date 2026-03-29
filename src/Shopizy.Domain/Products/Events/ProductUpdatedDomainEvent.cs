using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Products.Events;

public record ProductUpdatedDomainEvent(Product Product) : IDomainEvent;
