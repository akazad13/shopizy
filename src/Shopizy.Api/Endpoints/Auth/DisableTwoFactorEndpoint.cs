using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.Users.Commands.DisableTwoFactor;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Auth;

public class DisableTwoFactorEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/auth/2fa/disable",
            async (
                [FromServices] ICurrentUser currentUser,
                [FromServices] IDispatcher mediator,
                ILogger<DisableTwoFactorEndpoint> logger
            ) =>
            {
                var command = new DisableTwoFactorCommand(currentUser.GetCurrentUserId());

                return await HandleAsync(
                    mediator,
                    command,
                    _ => Results.Ok(SuccessResult.Success("Two-factor authentication has been disabled.")),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization()
        .WithTags("Auth")
        .WithSummary("Disable two-factor authentication")
        .WithDescription("Disables two-factor authentication for the authenticated user.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
