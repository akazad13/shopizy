using Shopizy.SharedKernel.Application.Messaging;
using MapsterMapper;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Admin.Queries.GetDashboardMetrics;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Shopizy.Api.Endpoints.Admin;

public class GetDashboardMetricsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/dashboard/metrics", async ([FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetDashboardMetricsEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new GetDashboardMetricsQuery(),
                metrics => Results.Ok(mapper.Map<DashboardMetricsResponse>(metrics)),
                ex => logger.LogError(ex, "Error fetching dashboard metrics")
            );
        })
        .RequireAuthorization("Admin.View")
        .WithTags("Dashboard")
        .WithSummary("Get dashboard metrics")
        .WithDescription("Retrieves key performance indicators and alerts for the admin dashboard.")
        .Produces<DashboardMetricsResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
