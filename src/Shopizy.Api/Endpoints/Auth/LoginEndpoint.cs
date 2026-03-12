using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Auth.Queries.login;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Auth;

/// <summary>
/// Endpoint for user login.
/// </summary>
public class LoginEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/auth/login", async (LoginRequest request, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<LoginEndpoint> logger) =>
        {
            var query = mapper.Map<LoginQuery>(request);

            return await HandleAsync(
                mediator,
                query,
                result => Results.Ok(mapper.Map<AuthResponse>(result)),
                ex => logger.UserLoginError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Auth")
        .WithOpenApi(operation =>
        {
            operation.Summary = "User login";
            operation.Description = "Authenticates a user and returns an access token.";
            return operation;
        })
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
