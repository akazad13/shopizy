using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class ProductVariant
    {
        public static Error VariantNotFound =>
            Error.NotFound(
                code: "ProductVariant.VariantNotFound",
                description: "Product variant is not found."
            );

        public static Error VariantNotCreated =>
            Error.Unexpected(
                code: "ProductVariant.VariantNotCreated",
                description: "Failed to create product variant."
            );
    }
}
