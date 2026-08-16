using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Returns.Commands.ApproveReturn;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Returns;

public class ApproveReturnEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut(
                "api/v1.0/returns/{returnId:guid}/approve",
                async (
                    Guid returnId,
                    [FromServices] IDispatcher mediator,
                    ILogger<ApproveReturnEndpoint> logger
                ) =>
                {
                    var command = new ApproveReturnCommand(returnId);

                    return await HandleAsync(
                        mediator,
                        command,
                        _ =>
                            Results.Ok(
                                SuccessResult.Success(
                                    "Return request approved and refund initiated."
                                )
                            ),
                        ex => logger.LogError(ex, "Error approving return request.")
                    );
                }
            )
            .RequireAuthorization("Order.Manage")
            .WithTags("Returns")
            .WithSummary("Approve a return request (Admin)")
            .WithDescription("Approves a pending return request and triggers a refund.")
            .Produces<SuccessResult>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
