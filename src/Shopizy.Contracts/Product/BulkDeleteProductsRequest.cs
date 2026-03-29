namespace Shopizy.Contracts.Product;

public record BulkDeleteProductsRequest(IList<Guid> ProductIds);
