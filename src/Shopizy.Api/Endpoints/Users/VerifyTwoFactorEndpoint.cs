using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.VerifyTwoFactor;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class VerifyTwoFactorEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/users/{userId:guid}/two-factor/verify",
            async (
                Guid userId,
                VerifyTwoFactorRequest request,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                ILogger<VerifyTwoFactorEndpoint> logger
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
                    new VerifyTwoFactorCommand(userId, request.Code),
                    _ => Results.Ok(SuccessResult.Success("Two-factor authentication has been confirmed.")),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Verify two-factor authentication")
        .WithDescription("Verifies the TOTP code and confirms two-factor authentication is enabled.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
