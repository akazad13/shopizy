namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Product
    {
        public static string DuplicateName => "Product with the same name is already in use.";
        public static string ProductNotFound => "Product is not found.";
        public static string ProductNotCreated => "Failed to create Product.";
        public static string ProductNotUpdated => "Failed to update Product.";
        public static string ProductNotDeleted => "Failed to delete Product.";
        public static string ProductImageNotAdded => "Failed to add Product image.";
        public static string ProductImageNotUploaded => "Please upload a valid Product image.";
        public static string ProductImageNotFound => "Product image is not found.";
    }
}
