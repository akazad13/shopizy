namespace Shopizy.Application.Admin.Queries.GetSalesReport;

public record SalesReportDto(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalRevenue,
    int TotalOrders,
    IReadOnlyList<TopProductDto> TopProducts
);
