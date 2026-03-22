using Shopizy.SharedKernel.Application.Messaging;
using MapsterMapper;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;

namespace Shopizy.Api.Endpoints.Orders;

public class GetAdminOrdersEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/orders", async ([AsParameters] OrdersCriteria ordersCriteria, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetAdminOrdersEndpoint> logger) =>
        {
            var query = new GetOrdersQuery(
                null,
                ordersCriteria.StartDate,
                ordersCriteria.EndDate,
                ordersCriteria.Status.HasValue ? (Domain.Orders.Enums.OrderStatus?)(int)ordersCriteria.Status.Value : null,
                ordersCriteria.PageNumber,
                ordersCriteria.PageSize
            );

            return await HandleAsync(
                mediator,
                query,
                orders => Results.Ok(mapper.Map<List<OrderResponse>>(orders)),
                ex => logger.AdminOrdersListFetchError(ex)
            );
        })
        .RequireAuthorization("Admin.ViewOrders")
        .WithTags("Orders")
        .WithSummary("List all orders")
        .WithDescription("Retrieves a paginated list of all orders in the system for administrative purposes.")
        .Produces<List<OrderResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
