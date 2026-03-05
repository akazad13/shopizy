using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Auth.Queries.login;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Auth;

public class LoginEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/auth/login", async (LoginRequest request, ISender mediator, IMapper mapper, ILogger<LoginEndpoint> logger) =>
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
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
