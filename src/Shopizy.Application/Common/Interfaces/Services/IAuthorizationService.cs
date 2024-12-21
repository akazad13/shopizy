using ErrorOr;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IAuthorizationService
{
    ErrorOr<Success> AuthorizeCurrentUser(
        IList<string> requiredRoles,
        IList<string> requiredPermissions,
        IList<string> requiredPolicies
    );
}
