namespace Shopizy.Contracts.Product;

/// <summary>
/// Represents a request to add a variant to a product.
/// </summary>
/// <param name="Name">The variant name (e.g. "Size: XL / Color: Red").</param>
/// <param name="SKU">The stock keeping unit.</param>
/// <param name="UnitPrice">The unit price amount.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="StockQuantity">The initial stock quantity.</param>
public record AddVariantRequest(
    string Name,
    string SKU,
    decimal UnitPrice,
    string Currency,
    int StockQuantity
);
