namespace Shopizy.Contracts.Admin;

public record DashboardMetricsResponse(
    decimal TotalRevenue,
    int TotalOrders,
    int TotalUsers,
    int TotalProducts,
    List<StockAlertResponse> StockAlerts
);

public record StockAlertResponse(
    Guid ProductId,
    string ProductName,
    int CurrentStock
);
