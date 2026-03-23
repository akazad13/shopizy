using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class ProductReview
    {
        public static Error ReviewNotFound =>
            Error.NotFound(code: "ProductReview.ReviewNotFound", description: "Product review is not found.");
        public static Error ReviewNotCreated =>
            Error.Failure(code: "ProductReview.ReviewNotCreated", description: "Failed to create product review.");
        public static Error DuplicateReview =>
            Error.Conflict(code: "ProductReview.DuplicateReview", description: "You have already reviewed this product.");
    }
}
