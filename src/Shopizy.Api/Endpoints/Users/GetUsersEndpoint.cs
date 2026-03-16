using Shopizy.SharedKernel.Application.Messaging;
using MapsterMapper;
using Shopizy.Application.Users.Queries.GetUsers;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Microsoft.AspNetCore.Mvc;

namespace Shopizy.Api.Endpoints.Users;

public class GetUsersEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/users", async (int pageNumber, int pageSize, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetUsersEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new GetUsersQuery(pageNumber, pageSize),
                users => Results.Ok(mapper.Map<List<UserDetails>>(users)),
                ex => logger.LogError(ex, "Error fetching users list for admin")
            );
        })
        .RequireAuthorization("Admin.ViewUsers")
        .WithTags("Users")
        .WithSummary("List all users")
        .WithDescription("Retrieves a paginated list of all registered users for administrative purposes.")
        .Produces<List<UserDetails>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
