namespace Shopizy.Contracts.Admin;

/// <summary>
/// Aggregate sales report for a closed date range.
/// </summary>
/// <param name="StartDate">Inclusive start of the report window (UTC).</param>
/// <param name="EndDate">Inclusive end of the report window (UTC).</param>
/// <param name="TotalRevenue">Sum of revenue across all paid orders in the window.</param>
/// <param name="TotalOrders">Total order count in the window.</param>
/// <param name="TopProducts">Top-selling products in the window, ranked by revenue.</param>
public record SalesReportResponse(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalRevenue,
    int TotalOrders,
    IReadOnlyList<TopProductResponse> TopProducts
);
