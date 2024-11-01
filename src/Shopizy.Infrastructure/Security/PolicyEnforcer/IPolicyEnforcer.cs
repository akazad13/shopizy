using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Infrastructure.Security.CurrentUserProvider;

namespace Shopizy.Infrastructure.Security.PolicyEnforcer;

public interface IPolicyEnforcer
{
    public IResult<GenericResponse> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy
    );
}
