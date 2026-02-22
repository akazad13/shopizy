using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Carts;

/// <summary>
/// Represents a shopping cart in the system.
/// </summary>
public sealed class Cart : AggregateRoot<CartId, Guid>
{
    private readonly List<CartItem> _cartItems = [];
    
    /// <summary>
    /// Gets the user ID who owns this cart.
    /// </summary>
    public UserId UserId { get; }
    
    /// <summary>
    /// Gets the date and time when the cart was created.
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the cart was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; private set; }
    
    /// <summary>
    /// Gets the read-only list of items in the cart.
    /// </summary>
    public IReadOnlyList<CartItem> CartItems => _cartItems.AsReadOnly();

    /// <summary>
    /// Creates a new shopping cart for a user.
    /// </summary>
    /// <param name="userId">The user ID who owns the cart.</param>
    /// <returns>A new <see cref="Cart"/> instance.</returns>
    public static Cart Create(UserId userId)
    {
        return new Cart(CartId.CreateUnique(), userId);
    }

    /// <summary>
    /// Adds an item to the cart.
    /// </summary>
    /// <param name="lineItem">The cart item to add.</param>
    public void AddLineItem(CartItem lineItem)
    {
        _cartItems.Add(lineItem);
        this.AddDomainEvent(new Events.CartItemAddedDomainEvent(this, lineItem));
    }

    /// <summary>
    /// Removes an item from the cart.
    /// </summary>
    /// <param name="lineItem">The cart item to remove.</param>
    public void RemoveLineItem(CartItem lineItem)
    {
        _cartItems.Remove(lineItem);
        ModifiedOn = DateTime.UtcNow;
        this.AddDomainEvent(new Events.CartItemRemovedDomainEvent(this, lineItem));
    }

    /// <summary>
    /// Updates the quantity of a cart item.
    /// </summary>
    /// <param name="cartItemId">The cart item identifier.</param>
    /// <param name="quantity">The new quantity.</param>
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
