using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.ForgotPassword;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Auth;

public class ForgotPasswordEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/auth/forgot-password",
            async (
                ForgotPasswordRequest request,
                [FromServices] IDispatcher mediator,
                ILogger<ForgotPasswordEndpoint> logger
            ) =>
            {
                var command = new ForgotPasswordCommand(request.Email);

                return await HandleAsync(
                    mediator,
                    command,
                    token => Results.Ok(new ForgotPasswordResponse(token)),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .AllowAnonymous()
        .WithTags("Auth")
        .WithSummary("Forgot password")
        .WithDescription("Initiates the password reset flow by generating a reset token.")
        .Produces<ForgotPasswordResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
