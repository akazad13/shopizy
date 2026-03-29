namespace Shopizy.Contracts.Wishlist;

public record WishlistResponse(
    Guid WishlistId,
    Guid UserId,
    string? Name,
    bool IsPublic,
    DateTime CreatedOn,
    DateTime? ModifiedOn,
    IList<WishlistItemResponse> WishlistItems
);

public record WishlistItemResponse(Guid WishlistItemId, Guid ProductId);
