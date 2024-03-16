using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Infrastructure.Security.PolicyEnforcer;
using Shopizy.Infrastructure.Security.CurrentUserProvider;

namespace Shopizy.Infrastructure.Security;

public class AuthorizationService(IPolicyEnforcer _policyEnforcer, ICurrentUserProvider _currentUserProvider) : IAuthorizationService
{
    public ErrorOr<Success> AuthorizeCurrentUser<T>(IAuthorizeableRequest<T> request, List<string> requiredRoles, List<string> requiredPermissions, List<string> requiredPolicies)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();

        if(currentUser == null)
            return Error.Unauthorized(description: "User is unauthorized.");

        if(requiredPermissions.Except(currentUser.Permissions).Any())
            return Error.Unauthorized(description: "User is missing required permissions for taking this action");

        if (requiredRoles.Except(currentUser.Roles).Any())
            return Error.Unauthorized(description: "User is missing required roles for taking this action");

        foreach( var policy in requiredPolicies)
        {
            var authorizationAgaistPolicyResult = _policyEnforcer.Authorize(request, currentUser, policy);
            if(authorizationAgaistPolicyResult.IsError)
            {
                return authorizationAgaistPolicyResult.Errors;
            }
        }
        return Result.Success;
    }
}