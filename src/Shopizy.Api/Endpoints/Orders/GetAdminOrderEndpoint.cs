using Shopizy.SharedKernel.Application.Messaging;
using MapsterMapper;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Microsoft.AspNetCore.Mvc;

namespace Shopizy.Api.Endpoints.Orders;

public class GetAdminOrderEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/orders/{id:guid}", async (Guid id, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetAdminOrderEndpoint> logger) =>
        {
            // Passing null userId to indicate admin view (if GetOrderQuery supports it)
            var query = new GetOrderQuery(Guid.Empty, id);

            return await HandleAsync(
                mediator,
                query,
                order => Results.Ok(mapper.Map<OrderDetailResponse>(order)),
                ex => logger.LogError(ex, "Error fetching order detail for admin")
            );
        })
        .RequireAuthorization("Admin.ViewOrder")
        .WithTags("Orders")
        .WithSummary("Get order detail")
        .WithDescription("Retrieves full details of a specific order for administrative purposes.")
        .Produces<OrderDetailResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
