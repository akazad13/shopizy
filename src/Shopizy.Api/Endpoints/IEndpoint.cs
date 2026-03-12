namespace Shopizy.Api.Endpoints;

/// <summary>
/// Defines a contract for mapping an API endpoint.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the provided route builder.
    /// </summary>
    /// <param name="app">The route builder.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
