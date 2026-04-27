namespace Shopizy.Contracts.Admin;

/// <summary>
/// One row in the "top customers by spend" report.
/// </summary>
/// <param name="UserId">Identifier of the customer.</param>
/// <param name="FirstName">Customer first name.</param>
/// <param name="LastName">Customer last name.</param>
/// <param name="TotalSpend">Lifetime spend across all paid orders.</param>
public record TopCustomerResponse(Guid UserId, string FirstName, string LastName, decimal TotalSpend);
