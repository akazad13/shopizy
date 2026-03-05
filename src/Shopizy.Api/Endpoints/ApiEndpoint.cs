using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Shopizy.Api.Endpoints;

/// <summary>
/// Base class for Minimal API endpoints to reduce boilerplate.
/// </summary>
public abstract class ApiEndpoint : IEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    /// <summary>
    /// Handles the request by sending a command/query through MediatR and matching the result.
    /// </summary>
    protected async Task<IResult> HandleAsync<TResponse>(
        ISender mediator,
        IRequest<ErrorOr<TResponse>> request,
        Func<TResponse, IResult> onSuccess,
        Action<Exception> onError)
    {
        try
        {
            var result = await mediator.Send(request);
            return result.Match(
                onSuccess,
                errors => CustomResults.Problem(errors)
            );
        }
        catch (Exception ex)
        {
            onError(ex);
            return CustomResults.Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
