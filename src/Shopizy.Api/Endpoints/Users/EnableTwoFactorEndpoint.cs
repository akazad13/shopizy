using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.EnableTwoFactor;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class EnableTwoFactorEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/users/{userId:guid}/two-factor/enable",
            async (
                Guid userId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                ILogger<EnableTwoFactorEndpoint> logger
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
                    new EnableTwoFactorCommand(userId),
                    setup => Results.Ok(new TwoFactorSetupResponse(setup.Secret, setup.QrCodeUri)),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Enable two-factor authentication")
        .WithDescription("Generates a TOTP secret and QR code URI to set up two-factor authentication.")
        .Produces<TwoFactorSetupResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
