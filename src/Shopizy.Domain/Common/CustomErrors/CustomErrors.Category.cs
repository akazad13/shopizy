namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Category
    {
        public static string DuplicateName => "Category with the same name is already in use.";
        public static string CategoryNotFound => "Category is not found.";
        public static string CategoryNotCreated => "Failed to create Category.";
        public static string CategoryNotUpdated => "Failed to update Category.";
        public static string CategoryNotDeleted => "Failed to delete Category.";
    }
}
