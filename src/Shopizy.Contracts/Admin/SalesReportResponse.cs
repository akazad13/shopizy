namespace Shopizy.Contracts.Admin;

public record SalesReportResponse(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalRevenue,
    int TotalOrders,
    IReadOnlyList<TopProductResponse> TopProducts
);
