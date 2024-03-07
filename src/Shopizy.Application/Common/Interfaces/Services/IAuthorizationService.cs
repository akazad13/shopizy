using ErrorOr;
using shopizy.Application.Common.Security.Request;

namespace shopizy.Application.Common.Interfaces.Services;

public interface IAuthorizationService
{
    ErrorOr<Success> AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        List<string> requiredRoles,
        List<string> requiredPermissions,
        List<string> requiredPolicies
    );
}
