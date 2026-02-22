using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

namespace Shopizy.Api.Endpoints.Users;

public class UpdateUserAddressEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/address", async (Guid userId, UpdateAddressRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<UpdateUserAddressEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to update this user's address.")]);
            }

            var command = mapper.Map<UpdateAddressCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated address.")),
                ex => logger.UserAddressUpdateError(ex)
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
