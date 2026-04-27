namespace Shopizy.Contracts.Product;

/// <summary>
/// Admin bulk-delete of products by id.
/// </summary>
/// <param name="ProductIds">Identifiers of the products to delete.</param>
public record BulkDeleteProductsRequest(IList<Guid> ProductIds);
