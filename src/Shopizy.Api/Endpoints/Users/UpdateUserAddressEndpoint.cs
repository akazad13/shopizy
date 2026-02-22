using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

namespace Shopizy.Api.Endpoints.Users;

public class UpdateUserAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/address", async (Guid userId, UpdateAddressRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<UpdateUserAddressEndpoint> logger) =>
        {
            try
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to update this user's address.")]);
                }

                var command = mapper.Map<UpdateAddressCommand>((userId, request));
                var result = await mediator.Send(command);

                return result.Match(
                    success => Results.Ok(SuccessResult.Success("Successfully updated address.")),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.UserAddressUpdateError(ex);
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
