using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.DisableTwoFactor;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class DisableTwoFactorEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "api/v1.0/users/{userId:guid}/two-factor",
            async (
                Guid userId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                ILogger<DisableTwoFactorEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to modify this user's two-factor settings.")]
                    );
                }

                return await HandleAsync(
                    mediator,
                    new DisableTwoFactorCommand(userId),
                    _ => Results.Ok(SuccessResult.Success("Two-factor authentication has been disabled.")),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Disable two-factor authentication")
        .WithDescription("Disables two-factor authentication for the specified user.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
