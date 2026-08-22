using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Products.Events;

/// <summary>
/// Domain event emitted when a product's effective price is reduced.
/// </summary>
public record ProductPriceDroppedDomainEvent(
    Product Product,
    decimal OldEffectivePrice,
    decimal NewEffectivePrice
) : IDomainEvent;
