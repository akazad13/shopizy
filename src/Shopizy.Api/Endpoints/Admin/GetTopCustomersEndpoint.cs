using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Admin.Queries.GetTopCustomers;
using Shopizy.Contracts.Admin;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Admin;

public class GetTopCustomersEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/reports/customers/top", async (
            [FromQuery] int count,
            [FromServices] IDispatcher mediator,
            IMapper mapper,
            ILogger<GetTopCustomersEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new GetTopCustomersQuery(count),
                customers => Results.Ok(mapper.Map<List<TopCustomerResponse>>(customers)),
                ex => logger.UserFetchError(ex)
            );
        })
        .RequireAuthorization("Admin.Reports")
        .WithTags("Admin Reports")
        .WithSummary("Get top customers by spend")
        .WithDescription("Retrieves the top customers ranked by total spend.")
        .Produces<List<TopCustomerResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
