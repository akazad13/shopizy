using ErrorOr;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Roles;
using Shopizy.Infrastructure.Security.CurrentUserProvider;

namespace Shopizy.Infrastructure.Security.PolicyEnforcer;

public class PolicyEnforcer : IPolicyEnforcer
{
    public ErrorOr<Success> Authorize(CurrentUser currentUser, string policy)
    {
        return policy switch
        {
            Policy.Admin => AdminPolicy(currentUser),
            _ => Error.Unexpected(description: "Unknown policy name."),
        };
    }

    private static ErrorOr<Success> AdminPolicy(CurrentUser currentUser)
    {
        return currentUser.Roles.Contains(Role.Admin)
            ? Result.Success
            : Error.Unauthorized(description: "Requesting user failed policy requirement.");
    }
}
