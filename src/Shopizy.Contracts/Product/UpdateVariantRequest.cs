namespace Shopizy.Contracts.Product;

/// <summary>
/// Represents a request to update a product variant.
/// </summary>
/// <param name="Name">The variant name.</param>
/// <param name="SKU">The stock keeping unit.</param>
/// <param name="UnitPrice">The unit price amount.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="StockQuantity">The stock quantity.</param>
/// <param name="IsActive">Whether the variant is active.</param>
public record UpdateVariantRequest(
    string Name,
    string SKU,
    decimal UnitPrice,
    string Currency,
    int StockQuantity,
    bool IsActive
);
