namespace Shopizy.Contracts.Admin;

/// <summary>
/// One row in the "top products by revenue" report.
/// </summary>
/// <param name="Name">Product display name.</param>
/// <param name="TotalQuantity">Total quantity sold in the window.</param>
/// <param name="Revenue">Total revenue from the product in the window.</param>
public record TopProductResponse(string Name, int TotalQuantity, decimal Revenue);
