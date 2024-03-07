using ErrorOr;
using shopizy.Application.Common.Security.Policies;
using shopizy.Application.Common.Security.Request;
using shopizy.Application.Common.Security.Roles;
using Shopizy.Infrastructure.Security.CurrentUserProvider;

namespace shopizy.Infrastructure.Security.PolicyEnforcer;

public class PolicyEnforcer : IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy
    )
    {
        return policy switch
        {
            Policy.SelfOrAdmin => SelfOrAdminPolicy(request, currentUser),
            _ => Error.Unexpected(description: "Unknown policy name.")
        };
    }

    private static ErrorOr<Success> SelfOrAdminPolicy<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser
    )
    {
        return request.UserId == currentUser.Id || currentUser.Roles.Contains(Role.Admin)
            ? Result.Success
            : Error.Unauthorized(description: "Requesting user failed policy requirement.");
    }
}
