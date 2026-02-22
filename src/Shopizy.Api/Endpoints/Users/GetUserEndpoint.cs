using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

namespace Shopizy.Api.Endpoints.Users;

public class GetUserEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/users/{userId:guid}", async (Guid userId, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<GetUserEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId)) 
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to access this user's information.")]);
            }

            var query = mapper.Map<GetUserQuery>(userId);

            return await HandleAsync(
                mediator,
                query,
                userResult => Results.Ok(mapper.Map<UserDetails>(userResult)),
                ex => logger.UserFetchError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Users")
        .Produces<UserDetails>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
