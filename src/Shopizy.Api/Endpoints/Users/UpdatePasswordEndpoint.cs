using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

namespace Shopizy.Api.Endpoints.Users;

public class UpdatePasswordEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/password", async (Guid userId, UpdatePasswordRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<UpdatePasswordEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to update this user's password.")]);
            }

            var command = mapper.Map<UpdatePasswordCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated password.")),
                ex => logger.UserPasswordUpdateError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Users")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
