using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Returns.Commands.RequestReturn;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Returns;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Returns;

public class RequestReturnEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                "api/v1.0/orders/{orderId:guid}/returns",
                async (
                    Guid orderId,
                    RequestReturnRequest request,
                    ClaimsPrincipal user,
                    [FromServices] IDispatcher mediator,
                    IMapper mapper,
                    ILogger<RequestReturnEndpoint> logger
                ) =>
                {
                    var userId = user.GetUserId();
                    if (userId is null)
                        return Results.Unauthorized();

                    var command = mapper.Map<RequestReturnCommand>(
                        (userId.Value, orderId, request)
                    );

                    return await HandleAsync(
                        mediator,
                        command,
                        returnId =>
                            Results.Ok(
                                SuccessResult.Success($"Return request submitted. ID: {returnId}")
                            ),
                        ex => logger.ReturnCreationError(ex)
                    );
                }
            )
            .RequireAuthorization("Order.Read")
            .WithTags("Returns")
            .WithSummary("Request a return for an order")
            .WithDescription("Submits a return request for a delivered order.")
            .Produces<SuccessResult>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
