namespace Shopizy.Contracts.Product;

public record BulkUpdateProductStatusRequest(IList<Guid> ProductIds, bool IsActive);
