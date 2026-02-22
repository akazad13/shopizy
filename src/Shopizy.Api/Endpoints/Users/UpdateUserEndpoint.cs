using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

namespace Shopizy.Api.Endpoints.Users;

public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1.0/users/{userId:guid}", async (Guid userId, UpdateUserRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<UpdateUserEndpoint> logger) =>
        {
            try
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to update this user's information.")]);
                }

                var command = mapper.Map<UpdateUserCommand>((userId, request));
                var result = await mediator.Send(command);

                return result.Match(
                    success => Results.Ok(SuccessResult.Success("Successfully updated user.")),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.UserUpdateError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
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
