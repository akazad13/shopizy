using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Cart
    {
        public static Error CartNotFound =>
            Error.Validation(code: "Cart.CartNotFound", description: "Cart is not found.");
        public static Error CartNotCreated =>
            Error.Failure(code: "Cart.CartNotCreated", description: "Failed to create Cart.");
        public static Error CartNotDeleted =>
            Error.Failure(code: "Cart.CartNotDeleted", description: "Failed to delete Cart.");
        public static Error CartProductNotAdded =>
            Error.Failure(
                code: "Cart.CartProductNotAdded",
                description: "Failed to add product to Cart."
            );
        public static Error CartProductNotFound =>
            Error.NotFound(
                code: "Cart.CartProductNotFound",
                description: "Cart product is not found."
            );
        public static Error CartNotUpdated =>
            Error.Failure(code: "Cart.CartNotUpdated", description: "Failed to update Cart.");
        public static Error CartProductNotRemoved =>
            Error.Failure(
                code: "Cart.CartProductNotRemoved",
                description: "Failed to remove product from Cart."
            );
        public static Error ProductAlreadyExistInCart =>
            Error.Failure(
                code: "Cart.ProductAlreadyExistInCart",
                description: "Product is already exist in Cart."
            );
    }
}
