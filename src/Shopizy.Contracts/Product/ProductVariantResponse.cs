namespace Shopizy.Contracts.Product;

/// <summary>
/// Represents a product variant.
/// </summary>
/// <param name="VariantId">The unique identifier of the variant.</param>
/// <param name="Name">The variant name (e.g. "Size: XL / Color: Red").</param>
/// <param name="SKU">The stock keeping unit.</param>
/// <param name="UnitPrice">The unit price amount.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="StockQuantity">The available stock quantity.</param>
/// <param name="IsActive">Whether the variant is active.</param>
public record ProductVariantResponse(
    Guid VariantId,
    string Name,
    string SKU,
    decimal UnitPrice,
    string Currency,
    int StockQuantity,
    bool IsActive
);
