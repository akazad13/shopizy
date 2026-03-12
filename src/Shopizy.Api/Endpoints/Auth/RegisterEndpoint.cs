using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Auth.Commands.Register;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Auth;

/// <summary>
/// Endpoint for user registration.
/// </summary>
public class RegisterEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/auth/register", async (RegisterRequest request, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<RegisterEndpoint> logger) =>
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
        .WithSummary("User registration")
        .WithDescription("Registers a new user in the system.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
