using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IAuthorizationService
{
    IResult<GenericResponse> AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        IList<string> requiredRoles,
        IList<string> requiredPermissions,
        IList<string> requiredPolicies
    );
}
