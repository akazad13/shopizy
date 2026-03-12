using System.Security.Claims;
using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Users;

public class UpdateUserAddressEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/address", async (Guid userId, UpdateAddressRequest request, ClaimsPrincipal user, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<UpdateUserAddressEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to update this user's address.")]);
            }

            var command = mapper.Map<UpdateAddressCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated address.")),
                ex => logger.UserAddressUpdateError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Users")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update user address";
            operation.Description = "Updates the mailing address of the authorized user.";
            return operation;
        })
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
