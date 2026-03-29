namespace Shopizy.Contracts.Order;

public record BulkUpdateOrderStatusRequest(IList<Guid> OrderIds, int Status);
