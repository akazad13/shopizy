namespace Shopizy.Contracts.Wishlist;

public record CreateWishlistRequest(string? Name = null, bool IsPublic = false);
