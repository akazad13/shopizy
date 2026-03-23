using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Contracts.Admin;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Admin;

public class GetSalesReportEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/reports/sales", async (
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromServices] IDispatcher mediator,
            IMapper mapper,
            ILogger<GetSalesReportEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new GetSalesReportQuery(startDate, endDate),
                report => Results.Ok(mapper.Map<SalesReportResponse>(report)),
                ex => logger.OrderFetchError(ex)
            );
        })
        .RequireAuthorization("Admin.Reports")
        .WithTags("Admin Reports")
        .WithSummary("Get sales report")
        .WithDescription("Retrieves a sales report for the specified date range.")
        .Produces<SalesReportResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
