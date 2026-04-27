namespace Shopizy.Contracts.Order;

/// <summary>
/// Admin bulk-update of order statuses.
/// </summary>
/// <param name="OrderIds">Identifiers of the orders to update.</param>
/// <param name="Status">Target status as the underlying <see cref="OrderStatus"/> integer value.</param>
public record BulkUpdateOrderStatusRequest(IList<Guid> OrderIds, int Status);
