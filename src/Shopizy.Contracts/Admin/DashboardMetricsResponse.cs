namespace Shopizy.Contracts.Admin;

/// <summary>
/// Aggregate metrics for the admin dashboard.
/// </summary>
/// <param name="TotalRevenue">Total revenue across all paid orders.</param>
/// <param name="TotalOrders">Total order count.</param>
/// <param name="TotalUsers">Total registered user count.</param>
/// <param name="TotalProducts">Total product count.</param>
/// <param name="StockAlerts">Products whose stock has dropped below the alert threshold.</param>
public record DashboardMetricsResponse(
    decimal TotalRevenue,
    int TotalOrders,
    int TotalUsers,
    int TotalProducts,
    IReadOnlyList<StockAlertResponse> StockAlerts
);

/// <summary>
/// Low-stock alert for a single product.
/// </summary>
/// <param name="ProductId">Identifier of the product.</param>
/// <param name="ProductName">Display name of the product.</param>
/// <param name="CurrentStock">Current on-hand quantity.</param>
public record StockAlertResponse(Guid ProductId, string ProductName, int CurrentStock);
