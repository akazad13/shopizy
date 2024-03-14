using ErrorOr;

namespace Shopizy.Domain.Common.Errors;

public static partial class Errors
{
    public static class Product
    {
        public static Error DuplicateName =>
            Error.Conflict(
                code: "Product.DuplicateName",
                description: "Product with the same name is already in use."
            );
        public static Error ProductNotFound =>
            Error.NotFound(code: "User.ProductNotFound", description: "Product is not found.");
        public static Error ProductNotCreated =>
            Error.Failure(
                code: "Product.ProductNotCreated",
                description: "Failed to create Product."
            );
    }
}
