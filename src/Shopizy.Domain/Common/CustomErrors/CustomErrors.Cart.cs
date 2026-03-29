using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Cart
    {
        public static DomainError CartNotFound =>
            DomainError.Validation(code: "Cart.CartNotFound", description: "Cart is not found.");
        public static DomainError CartNotCreated =>
            DomainError.Failure(code: "Cart.CartNotCreated", description: "Failed to create Cart.");
        public static DomainError CartNotDeleted =>
            DomainError.Failure(code: "Cart.CartNotDeleted", description: "Failed to delete Cart.");
        public static DomainError CartProductNotAdded =>
            DomainError.Failure(
                code: "Cart.CartProductNotAdded",
                description: "Failed to add product to Cart."
            );
        public static DomainError CartProductNotFound =>
            DomainError.NotFound(
                code: "Cart.CartProductNotFound",
                description: "Cart product is not found."
            );
        public static DomainError CartNotUpdated =>
            DomainError.Failure(code: "Cart.CartNotUpdated", description: "Failed to update Cart.");
        public static DomainError CartProductNotRemoved =>
            DomainError.Failure(
                code: "Cart.CartProductNotRemoved",
                description: "Failed to remove product from Cart."
            );
        public static DomainError ProductAlreadyExistInCart =>
            DomainError.Failure(
                code: "Cart.ProductAlreadyExistInCart",
                description: "Product is already exist in Cart."
            );
    }
}
