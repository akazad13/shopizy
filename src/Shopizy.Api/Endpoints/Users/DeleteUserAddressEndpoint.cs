using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.DeleteUserAddress;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class DeleteUserAddressEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "api/v1.0/users/{userId:guid}/addresses/{addressId:guid}",
            async (
                Guid userId,
                Guid addressId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                ILogger<DeleteUserAddressEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to delete this address.")]
                    );
                }

                var command = new DeleteUserAddressCommand(userId, addressId);

                return await HandleAsync(
                    mediator,
                    command,
                    _ => Results.NoContent(),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Delete user address")
        .WithDescription("Deletes an address from the authorized user's address book.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
