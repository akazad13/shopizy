namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Cart
    {
        public static string CartNotFound => "Cart is not found.";
        public static string CartNotCreated => "Failed to create Cart.";
        public static string CartNotDeleted => "Failed to delete Cart.";
        public static string CartPrductNotAdded => "Failed to add product to Cart.";
        public static string CartPrductNotFound => "Cart image is not found.";
        public static string CartNotUpdated => "Failed to update Cart.";
        public static string CartPrductNotRemoved => "Failed to remove product from Cart.";
        public static string ProductAlreadyExistInCart => "Product is already exist in Cart.";
    }
}
