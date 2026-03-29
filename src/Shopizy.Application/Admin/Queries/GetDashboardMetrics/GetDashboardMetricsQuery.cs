using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;

namespace Shopizy.Application.Admin.Queries.GetDashboardMetrics;

public record GetDashboardMetricsQuery() : IQuery<ErrorOr<DashboardMetricsDto>>;
