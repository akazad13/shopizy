using ErrorOr;
using shopizy.Application.Common.Security.Request;
using Shopizy.Infrastructure.Security.CurrentUserProvider;

namespace shopizy.Infrastructure.Security.PolicyEnforcer;

public interface IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy
    );
}
