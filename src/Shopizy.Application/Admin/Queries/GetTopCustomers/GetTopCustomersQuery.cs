using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Admin.Queries.GetSalesReport;

namespace Shopizy.Application.Admin.Queries.GetTopCustomers;

public record GetTopCustomersQuery(int Count = 10) : IQuery<ErrorOr<List<TopCustomerDto>>>;
