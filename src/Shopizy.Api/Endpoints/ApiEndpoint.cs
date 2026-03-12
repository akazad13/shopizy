using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints;

/// <summary>
/// Base class for Minimal API endpoints to reduce boilerplate.
/// </summary>
public abstract class ApiEndpoint : IEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    /// <summary>
    /// Handles the request by sending a command through the dispatcher and matching the result.
    /// </summary>
    protected async Task<IResult> HandleAsync<TResponse>(
        IDispatcher dispatcher,
        ICommand<ErrorOr<TResponse>> command,
        Func<TResponse, IResult> onSuccess,
        Action<Exception> onError)
    {
        try
        {
            var result = await dispatcher.SendAsync(command);
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

    /// <summary>
    /// Handles the request by sending a query through the dispatcher and matching the result.
    /// </summary>
    protected async Task<IResult> HandleAsync<TResponse>(
        IDispatcher dispatcher,
        IQuery<ErrorOr<TResponse>> query,
        Func<TResponse, IResult> onSuccess,
        Action<Exception> onError)
    {
        try
        {
            var result = await dispatcher.SendAsync(query);
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
