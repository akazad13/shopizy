using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Carts.Events;

public record CartItemAddedDomainEvent(Cart Cart, CartItem CartItem) : IDomainEvent;
