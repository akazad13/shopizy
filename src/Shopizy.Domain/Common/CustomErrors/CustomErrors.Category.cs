using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Category
    {
        public static Error DuplicateName =>
            Error.Conflict(
                code: "Category.DuplicateName",
                description: "Category with the same name is already in use."
            );
        public static string CategoryNotFound => "Category is not found.";
        public static Error CategoryNotCreated =>
            Error.Failure(
                code: "Category.CategoryNotCreated",
                description: "Failed to create Category."
            );
        public static string CategoryNotUpdated => "Failed to update Category.";
        public static string CategoryNotDeleted => "Failed to delete Category.";
    }
}
