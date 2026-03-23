using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.Users.Commands.EnableTwoFactor;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Auth;

public class EnableTwoFactorEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/auth/2fa/enable",
            async (
                [FromServices] ICurrentUser currentUser,
                [FromServices] IDispatcher mediator,
                ILogger<EnableTwoFactorEndpoint> logger
            ) =>
            {
                var command = new EnableTwoFactorCommand(currentUser.GetCurrentUserId());

                return await HandleAsync(
                    mediator,
                    command,
                    setup => Results.Ok(new TwoFactorSetupResponse(setup.Secret, setup.QrCodeUri)),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization()
        .WithTags("Auth")
        .WithSummary("Enable two-factor authentication")
        .WithDescription("Generates a TOTP secret and QR code URI to set up two-factor authentication.")
        .Produces<TwoFactorSetupResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
