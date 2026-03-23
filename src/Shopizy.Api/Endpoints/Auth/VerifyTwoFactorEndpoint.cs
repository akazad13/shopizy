using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.Users.Commands.VerifyTwoFactor;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Auth;

public class VerifyTwoFactorEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/auth/2fa/verify",
            async (
                VerifyTwoFactorRequest request,
                [FromServices] ICurrentUser currentUser,
                [FromServices] IDispatcher mediator,
                ILogger<VerifyTwoFactorEndpoint> logger
            ) =>
            {
                var command = new VerifyTwoFactorCommand(currentUser.GetCurrentUserId(), request.Code);

                return await HandleAsync(
                    mediator,
                    command,
                    _ => Results.Ok(SuccessResult.Success("Two-factor authentication has been confirmed.")),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization()
        .WithTags("Auth")
        .WithSummary("Verify two-factor authentication")
        .WithDescription("Verifies the TOTP code and confirms two-factor authentication is enabled.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
