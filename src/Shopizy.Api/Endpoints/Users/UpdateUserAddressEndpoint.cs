using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateUserAddress;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class UpdateUserAddressEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "api/v1.0/users/{userId:guid}/addresses/{addressId:guid}",
            async (
                Guid userId,
                Guid addressId,
                UpdateUserAddressRequest request,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<UpdateUserAddressEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to update this address.")]
                    );
                }

                var command = new UpdateUserAddressCommand(
                    userId,
                    addressId,
                    request.Street,
                    request.City,
                    request.State,
                    request.Country,
                    request.ZipCode
                );

                return await HandleAsync(
                    mediator,
                    command,
                    address => Results.Ok(mapper.Map<UserAddressResponse>(address)),
                    ex => logger.UserAddressUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Update user address")
        .WithDescription("Updates an existing address in the authorized user's address book.")
        .Produces<UserAddressResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
