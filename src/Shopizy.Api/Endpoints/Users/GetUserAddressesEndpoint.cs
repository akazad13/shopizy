using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Queries.GetUserAddresses;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Users;

public class GetUserAddressesEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/users/{userId:guid}/addresses",
            async (
                Guid userId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetUserAddressesEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to access these addresses.")]
                    );
                }

                var query = new GetUserAddressesQuery(userId);

                return await HandleAsync(
                    mediator,
                    query,
                    addresses => Results.Ok(mapper.Map<List<UserAddressResponse>>(addresses)),
                    ex => logger.UserFetchError(ex)
                );
            }
        )
        .RequireAuthorization("User.Get")
        .WithTags("Users")
        .WithSummary("Get user addresses")
        .WithDescription("Retrieves all addresses for the authorized user.")
        .Produces<List<UserAddressResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
