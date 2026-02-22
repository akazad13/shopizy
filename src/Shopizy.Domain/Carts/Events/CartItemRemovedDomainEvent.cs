using Shopizy.Domain.Carts.Entities;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Carts.Events;

public record CartItemRemovedDomainEvent(Cart Cart, CartItem CartItem) : IDomainEvent;
