using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Admin.Queries.ExportAnalytics;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Dashboard;

public class ExportAnalyticsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                "api/v1.0/admin/dashboard/export",
                async (
                    [FromQuery] string? format,
                    [FromServices] IDispatcher mediator,
                    ILogger<ExportAnalyticsEndpoint> logger
                ) =>
                {
                    var exportFormat = format ?? "csv";
                    var query = new ExportAnalyticsQuery(exportFormat);

                    var result = await mediator.SendAsync(query);
                    return result.Match(
                        file => Results.File(file.Content, file.ContentType, file.FileName),
                        errors => CustomResults.Problem(errors)
                    );
                }
            )
            .RequireAuthorization("Admin.View")
            .WithTags("Dashboard")
            .WithSummary("Export analytics report")
            .WithDescription("Exports administrative analytics summary data as CSV or PDF file.")
            .Produces(StatusCodes.Status200OK, contentType: "text/csv")
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
