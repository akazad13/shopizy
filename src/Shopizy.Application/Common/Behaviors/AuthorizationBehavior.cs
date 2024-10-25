using System.Reflection;
using MediatR;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(IAuthorizationService _authorizationService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizeableRequest<TResponse>
    where TResponse : IResult<GenericResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var authorizationAttributes = request
            .GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();
        if (authorizationAttributes.Count == 0)
        {
            return await next();
        }

        var requiredPermissions = authorizationAttributes
            .SelectMany(attr => attr.Permissions?.Split(',') ?? [])
            .ToList();
        var requiredRoles = authorizationAttributes
            .SelectMany(attr => attr.Roles?.Split(',') ?? [])
            .ToList();
        var requiredPolicies = authorizationAttributes
            .SelectMany(attr => attr.Policies?.Split(',') ?? [])
            .ToList();

        var authorizationResult = _authorizationService.AuthorizeCurrentUser(
            request,
            requiredRoles,
            requiredPermissions,
            requiredPolicies
        );

        return authorizationResult.Succeeded() ? await next() : (dynamic)authorizationResult;
    }
}
