namespace Shopizy.Application.Admin.Queries.GetDashboardMetrics;

public record DashboardMetricsDto(
    decimal TotalRevenue,
    int TotalOrders,
    int TotalUsers,
    int TotalProducts,
    List<StockAlertDto> StockAlerts
);

public record StockAlertDto(
    Guid ProductId,
    string ProductName,
    int CurrentStock
);
