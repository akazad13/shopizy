using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Users.Commands.UpdateUserRole;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Shopizy.Api.Endpoints.Users;

public class UpdateUserRoleEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/admin/users/{id:guid}/role", async (Guid id, [FromBody] UpdateUserRoleRequest request, [FromServices] IDispatcher mediator, ILogger<UpdateUserRoleEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new UpdateUserRoleCommand(id, request.Role, request.PermissionIds),
                success => Results.Ok(SuccessResult.Success("User roles/permissions updated successfully.")),
                ex => logger.LogError(ex, "Error updating user roles")
            );
        })
        .RequireAuthorization("Admin.UpdateUserRole")
        .WithTags("Users")
        .WithSummary("Update user roles")
        .WithDescription("Allows an admin to update the permissions/roles assigned to a specific user.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
