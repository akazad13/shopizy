using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Admin.Queries.GetDashboardMetrics;

public record GetDashboardMetricsQuery() : IQuery<ErrorOr<DashboardMetricsDto>>;
