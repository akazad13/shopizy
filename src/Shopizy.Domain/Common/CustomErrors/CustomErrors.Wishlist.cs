using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Wishlist
    {
        public static Error WishlistNotFound =>
            Error.Validation(code: "Wishlist.WishlistNotFound", description: "Wishlist is not found.");
        public static Error WishlistNotCreated =>
            Error.Failure(code: "Wishlist.WishlistNotCreated", description: "Failed to create Wishlist.");
        public static Error WishlistAlreadyExists =>
            Error.Conflict(code: "Wishlist.WishlistAlreadyExists", description: "Wishlist already exists for this user.");
        public static Error ProductAlreadyInWishlist =>
            Error.Conflict(code: "Wishlist.ProductAlreadyInWishlist", description: "Product is already in the Wishlist.");
        public static Error ProductNotInWishlist =>
            Error.Validation(code: "Wishlist.ProductNotInWishlist", description: "Product is not in the Wishlist.");
    }
}
