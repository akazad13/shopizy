using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class ProductVariant
    {
        public static DomainError VariantNotFound =>
            DomainError.NotFound(
                code: "ProductVariant.VariantNotFound",
                description: "Product variant is not found."
            );

        public static DomainError VariantNotCreated =>
            DomainError.Failure(
                code: "ProductVariant.VariantNotCreated",
                description: "Failed to create product variant."
            );
    }
}
