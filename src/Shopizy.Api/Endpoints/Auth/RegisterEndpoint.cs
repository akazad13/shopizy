using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Auth.Commands.Register;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Auth;

public class RegisterEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/auth/register", async (RegisterRequest request, ISender mediator, IMapper mapper, ILogger<RegisterEndpoint> logger) =>
        {
            var command = mapper.Map<RegisterCommand>(request);

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Your account has been added. Please log in.")),
                ex => logger.UserRegisterError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Auth")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
