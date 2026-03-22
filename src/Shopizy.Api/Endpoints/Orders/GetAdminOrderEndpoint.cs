using Shopizy.SharedKernel.Application.Messaging;
using MapsterMapper;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;

namespace Shopizy.Api.Endpoints.Orders;

public class GetAdminOrderEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/orders/{id:guid}", async (Guid id, [FromServices] ICurrentUser currentUser, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetAdminOrderEndpoint> logger) =>
        {
            var query = new GetOrderQuery(currentUser.GetCurrentUserId(), id);

            return await HandleAsync(
                mediator,
                query,
                order => Results.Ok(mapper.Map<OrderDetailResponse>(order)),
                ex => logger.AdminOrderDetailFetchError(ex)
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
