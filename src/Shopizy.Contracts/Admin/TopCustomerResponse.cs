namespace Shopizy.Contracts.Admin;

public record TopCustomerResponse(Guid UserId, string FirstName, string LastName, decimal TotalSpend);
