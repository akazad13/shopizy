using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.AddUserAddress;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class AddUserAddressEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/users/{userId:guid}/addresses",
            async (
                Guid userId,
                AddUserAddressRequest request,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<AddUserAddressEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to add an address for this user.")]
                    );
                }

                var command = new AddUserAddressCommand(
                    userId,
                    request.Street,
                    request.City,
                    request.State,
                    request.Country,
                    request.ZipCode,
                    request.IsDefault
                );

                return await HandleAsync(
                    mediator,
                    command,
                    address => Results.Created(
                        $"api/v1.0/users/{userId}/addresses/{address.Id.Value}",
                        mapper.Map<UserAddressResponse>(address)
                    ),
                    ex => logger.UserUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("User.Modify")
        .WithTags("Users")
        .WithSummary("Add user address")
        .WithDescription("Adds a new address to the authorized user's address book.")
        .Produces<UserAddressResponse>(StatusCodes.Status201Created)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
