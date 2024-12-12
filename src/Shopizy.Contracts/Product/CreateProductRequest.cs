namespace Shopizy.Contracts.Product;

public record CreateProductRequest(
    string Name,
    string ShortDescription,
    string Description,
    Guid CategoryId,
    decimal UnitPrice,
    int Currency,
    decimal Discount,
    string Sku,
    string Brand,
    string Colors,
    string Sizes,
    string Tags,
    string Barcode,
    IList<Guid>? SpecificationIds
);
