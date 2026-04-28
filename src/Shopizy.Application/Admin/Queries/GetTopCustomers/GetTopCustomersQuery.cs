using ErrorOr;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Admin.Queries.GetTopCustomers;

public record GetTopCustomersQuery(int Count = 10) : IQuery<ErrorOr<List<TopCustomerDto>>>;
