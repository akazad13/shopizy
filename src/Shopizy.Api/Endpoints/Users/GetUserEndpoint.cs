using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;

namespace Shopizy.Api.Endpoints.Users;

public class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/users/{userId:guid}", async (Guid userId, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<GetUserEndpoint> logger) =>
        {
            try
            {
                if (!user.IsAuthorized(userId)) 
                {
                    return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to access this user's information.")]);
                }

                var query = mapper.Map<GetUserQuery>(userId);
                var result = await mediator.Send(query);

                return result.Match(
                    userResult => Results.Ok(mapper.Map<UserDetails>(userResult)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.UserFetchError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
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
