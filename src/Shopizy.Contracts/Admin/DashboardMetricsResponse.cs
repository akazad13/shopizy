namespace Shopizy.Contracts.Admin;

public record DashboardMetricsResponse(
    decimal TotalRevenue,
    int TotalOrders,
    int TotalUsers,
    int TotalProducts,
    IReadOnlyList<StockAlertResponse> StockAlerts
);

public record StockAlertResponse(
    Guid ProductId,
    string ProductName,
    int CurrentStock
);
