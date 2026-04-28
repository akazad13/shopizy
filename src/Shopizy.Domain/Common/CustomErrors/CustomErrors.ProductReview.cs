using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class ProductReview
    {
        public static DomainError ReviewNotFound =>
            DomainError.NotFound(
                code: "ProductReview.ReviewNotFound",
                description: "Product review is not found."
            );
        public static DomainError ReviewNotCreated =>
            DomainError.Failure(
                code: "ProductReview.ReviewNotCreated",
                description: "Failed to create product review."
            );
        public static DomainError DuplicateReview =>
            DomainError.Conflict(
                code: "ProductReview.DuplicateReview",
                description: "You have already reviewed this product."
            );
    }
}
