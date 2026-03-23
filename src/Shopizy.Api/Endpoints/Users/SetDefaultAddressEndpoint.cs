using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.SetDefaultAddress;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class SetDefaultAddressEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "api/v1.0/users/{userId:guid}/addresses/{addressId:guid}/set-default",
            async (
                Guid userId,
                Guid addressId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                ILogger<SetDefaultAddressEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to set the default address.")]
                    );
                }

                var command = new SetDefaultAddressCommand(userId, addressId);

                return await HandleAsync(
                    mediator,
                    command,
                    _ => Results.Ok(SuccessResult.Success("Default address updated successfully.")),
                    ex => logger.UserAddressUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Set default address")
        .WithDescription("Sets the specified address as the default for the authorized user.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
