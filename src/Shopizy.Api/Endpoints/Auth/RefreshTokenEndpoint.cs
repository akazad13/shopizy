using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Auth.Commands.RefreshToken;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Auth;

public class RefreshTokenEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/auth/refresh", async (RefreshTokenRequest request, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<RefreshTokenEndpoint> logger) =>
        {
            var command = mapper.Map<RefreshTokenCommand>(request);

            return await HandleAsync(
                mediator,
                command,
                result => Results.Ok(mapper.Map<AuthResponse>(result)),
                ex => logger.UserLoginError(ex)
            );
        })
        .AllowAnonymous()
        .RequireRateLimiting("auth")
        .WithTags("Auth")
        .WithSummary("Refresh access token")
        .WithDescription("Exchanges a valid refresh token for a new access + refresh token pair. The supplied refresh token is rotated and may not be reused.")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
