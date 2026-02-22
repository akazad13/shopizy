using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Auth.Queries.login;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Auth;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/auth/login", async (LoginRequest request, ISender mediator, IMapper mapper, ILogger<LoginEndpoint> logger) =>
        {
            try
            {
                var query = mapper.Map<LoginQuery>(request);
                var authResult = await mediator.Send(query);

                return authResult.Match(
                    result => Results.Ok(mapper.Map<AuthResponse>(result)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.UserLoginError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
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
