using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Wishlist
    {
        public static DomainError WishlistNotFound =>
            DomainError.Validation(code: "Wishlist.WishlistNotFound", description: "Wishlist is not found.");
        public static DomainError WishlistNotCreated =>
            DomainError.Failure(code: "Wishlist.WishlistNotCreated", description: "Failed to create Wishlist.");
        public static DomainError WishlistAlreadyExists =>
            DomainError.Conflict(code: "Wishlist.WishlistAlreadyExists", description: "Wishlist already exists for this user.");
        public static DomainError ProductAlreadyInWishlist =>
            DomainError.Conflict(code: "Wishlist.ProductAlreadyInWishlist", description: "Product is already in the Wishlist.");
        public static DomainError ProductNotInWishlist =>
            DomainError.Validation(code: "Wishlist.ProductNotInWishlist", description: "Product is not in the Wishlist.");
    }
}
