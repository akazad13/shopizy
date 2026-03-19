using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists.Entities;
using Shopizy.Domain.Wishlists.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Wishlists;

public sealed class Wishlist : AggregateRoot<WishlistId, Guid>, IAuditable
{
    private readonly List<WishlistItem> _wishlistItems = [];

    public UserId UserId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }
    public IReadOnlyList<WishlistItem> WishlistItems => _wishlistItems.AsReadOnly();

    public static Wishlist Create(UserId userId)
    {
        return new Wishlist(WishlistId.CreateUnique(), userId);
    }

    public void AddItem(ProductId productId)
    {
        _wishlistItems.Add(WishlistItem.Create(productId));
    }

    public void RemoveItem(ProductId productId)
    {
        var item = _wishlistItems.Find(i => i.ProductId == productId);
        if (item is not null)
        {
            _wishlistItems.Remove(item);
        }
    }

    private Wishlist(WishlistId id, UserId userId)
        : base(id)
    {
        UserId = userId;
    }

    private Wishlist() { }
}
