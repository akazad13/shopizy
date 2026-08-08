using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.Domain.Wishlists.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Wishlists;

public class WishlistTests
{
    [Fact]
    public void Create_AndUpdateSettings_ShouldWork()
    {
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Favorites", false);

        wishlist.ShouldNotBeNull();
        wishlist.UserId.ShouldBe(userId);
        wishlist.Name.ShouldBe("My Favorites");
        wishlist.IsPublic.ShouldBeFalse();

        wishlist.UpdateSettings("Public Favorites", true);

        wishlist.Name.ShouldBe("Public Favorites");
        wishlist.IsPublic.ShouldBeTrue();
    }

    [Fact]
    public void AddAndRemoveItem_ShouldManageWishlistItems()
    {
        var wishlist = Wishlist.Create(UserId.CreateUnique(), "List", true);
        var productId = ProductId.CreateUnique();

        wishlist.AddItem(productId);
        wishlist.WishlistItems.Count.ShouldBe(1);
        wishlist.WishlistItems[0].ProductId.ShouldBe(productId);

        wishlist.RemoveItem(productId);
        wishlist.WishlistItems.Count.ShouldBe(0);

        // Remove non-existent item should not fail
        wishlist.RemoveItem(ProductId.CreateUnique());
    }

    [Fact]
    public void WishlistId_And_WishlistItemId_CreateUniqueAndCreate_ShouldWork()
    {
        var wId1 = WishlistId.CreateUnique();
        var raw1 = Guid.NewGuid();
        var wId2 = WishlistId.Create(raw1);

        var wiId1 = WishlistItemId.CreateUnique();
        var raw2 = Guid.NewGuid();
        var wiId2 = WishlistItemId.Create(raw2);

        wId1.Value.ShouldNotBe(Guid.Empty);
        wId2.Value.ShouldBe(raw1);
        wiId1.Value.ShouldNotBe(Guid.Empty);
        wiId2.Value.ShouldBe(raw2);
    }
}
