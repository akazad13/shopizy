using ErrorOr;

namespace Shopizy.Domain.Common.Errors;

public static partial class Errors
{
    public static class Category
    {
        public static Error DuplicateName =>
            Error.Conflict(
                code: "Category.DuplicateName",
                description: "Category with the same name is already in use."
            );
        public static Error CategoryNotFound =>
            Error.NotFound(code: "User.CategoryNotFound", description: "Category is not found.");
        public static Error CategoryNotCreated =>
            Error.Failure(
                code: "Category.CategoryNotCreated",
                description: "Failed to create Category."
            );
    }
}
