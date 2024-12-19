using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Carts;

public sealed class Cart : AggregateRoot<CartId, Guid>
{
    private readonly List<CartItem> _cartItems = [];
    public UserId UserId { get; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }
    public IReadOnlyList<CartItem> CartItems => _cartItems.AsReadOnly();

    public static Cart Create(UserId userId)
    {
        return new Cart(CartId.CreateUnique(), userId);
    }

    public void AddLineItem(CartItem lineItem)
    {
        _cartItems.Add(lineItem);
    }

    public void RemoveLineItem(CartItem lineItem)
    {
        _cartItems.Remove(lineItem);
        ModifiedOn = DateTime.UtcNow;
    }

    public void UpdateLineItem(CartItemId cartItemId, int quantity)
    {
        _cartItems.Find(li => li.Id == cartItemId)?.UpdateQuantity(quantity);
        ModifiedOn = DateTime.UtcNow;
    }

    private Cart(CartId cartId, UserId userId)
        : base(cartId)
    {
        UserId = userId;
        CreatedOn = DateTime.UtcNow;
    }

    private Cart() { }
}
