using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Carts.Events;

public record CartItemRemovedDomainEvent(Cart Cart, CartItem CartItem) : IDomainEvent;
