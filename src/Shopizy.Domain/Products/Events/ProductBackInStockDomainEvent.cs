using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Products.Events;

/// <summary>
/// Domain event emitted when an out-of-stock product receives new inventory.
/// </summary>
public record ProductBackInStockDomainEvent(Product Product) : IDomainEvent;
