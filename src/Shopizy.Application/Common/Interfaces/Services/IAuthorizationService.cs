using ErrorOr;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IAuthorizationService
{
    ErrorOr<Success> AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        IList<string> requiredRoles,
        IList<string> requiredPermissions,
        IList<string> requiredPolicies
    );
}
