using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Admin.Queries.GetSalesReport;

namespace Shopizy.Application.Admin.Queries.GetTopProducts;

public record GetTopProductsQuery(int Count = 10) : IQuery<ErrorOr<List<TopProductDto>>>;
