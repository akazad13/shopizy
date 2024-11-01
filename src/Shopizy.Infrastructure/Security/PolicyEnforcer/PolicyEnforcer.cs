using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Roles;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Infrastructure.Security.CurrentUserProvider;

namespace Shopizy.Infrastructure.Security.PolicyEnforcer;

public class PolicyEnforcer : IPolicyEnforcer
{
    public IResult<GenericResponse> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy
    )
    {
        return policy switch
        {
            Policy.SelfOrAdmin => SelfOrAdminPolicy(request, currentUser),
            _ => Response<GenericResponse>.ErrorResponse(["Unknown policy name."]),
        };
    }

    private static IResult<GenericResponse> SelfOrAdminPolicy<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser
    )
    {
        return request.UserId == currentUser.Id || currentUser.Roles.Contains(Role.Admin)
            ? Response<GenericResponse>.SuccessResponese("")
            : Response<GenericResponse>.ErrorResponse(
                ["Requesting user failed policy requirement."]
            );
    }
}
