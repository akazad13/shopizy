namespace Shopizy.Contracts.Wishlist;

public record UpdateWishlistRequest(Guid ProductId, string Action);
