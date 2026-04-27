namespace Shopizy.Contracts.Product;

/// <summary>
/// Admin bulk-toggle of product active state.
/// </summary>
/// <param name="ProductIds">Identifiers of the products to update.</param>
/// <param name="IsActive">New active flag — products are visible to shoppers when true.</param>
public record BulkUpdateProductStatusRequest(IList<Guid> ProductIds, bool IsActive);
