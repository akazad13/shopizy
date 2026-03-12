using System.Security.Claims;
using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Users;

public class UpdateUserEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1.0/users/{userId:guid}", async (Guid userId, UpdateUserRequest request, ClaimsPrincipal user, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<UpdateUserEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to update this user's information.")]);
            }

            var command = mapper.Map<UpdateUserCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated user.")),
                ex => logger.UserUpdateError(ex)
            );
        })
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Update user profile")
        .WithDescription("Updates the profile information of the authorized user.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
