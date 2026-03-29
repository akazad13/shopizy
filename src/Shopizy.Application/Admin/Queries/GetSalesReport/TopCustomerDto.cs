namespace Shopizy.Application.Admin.Queries.GetSalesReport;

public record TopCustomerDto(Guid UserId, string FirstName, string LastName, decimal TotalSpend);
