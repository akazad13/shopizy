using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Admin.Queries.GetSalesReport;

public record GetSalesReportQuery(DateTime StartDate, DateTime EndDate) : IQuery<ErrorOr<SalesReportDto>>;
