using Moq;
using Shopizy.Api.Endpoints.ProductReviews;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Endpoints.ProductReviews;

/// <summary>
/// Unit tests for the GetProductReviewsEndpoint class.
/// </summary>
/// <remarks>
/// Note: The MapEndpoint method primarily consists of endpoint configuration using extension methods
/// that operate on concrete types (IEndpointRouteBuilder, RouteHandlerBuilder) which cannot be easily
/// mocked in isolation. The actual business logic (the lambda handler) delegates to the base class
/// HandleAsync method. Comprehensive testing of this endpoint is best achieved through integration tests
/// that invoke the actual HTTP endpoint, as demonstrated in the project's existing integration test suite.
///
/// These unit tests verify that the configuration method can be invoked without errors, but cannot
/// fully validate the routing configuration or the runtime behavior of the registered lambda handler
/// due to the architectural limitations of testing ASP.NET Core minimal API endpoint configuration.
/// </remarks>
public class GetProductReviewsEndpointTests
{
    /// <summary>
    /// Tests that MapEndpoint can be called with a valid IEndpointRouteBuilder without throwing an exception.
    /// </summary>
    /// <remarks>
    /// This test verifies basic invocation but cannot validate the full endpoint configuration due to
    /// the use of extension methods on concrete types that are not easily mockable. The method under test
    /// returns void and primarily performs configuration through a fluent API.
    ///
    /// For comprehensive validation of endpoint behavior including:
    /// - Route pattern matching
    /// - Parameter binding (productId, pageNumber, pageSize)
    /// - Query execution through IDispatcher
    /// - Response mapping and status codes
    /// - Error handling and logging
    ///
    /// Integration tests should be used that make actual HTTP requests to the configured endpoint.
    /// </remarks>
    [Fact]
    public void MapEndpoint_WithValidRouteBuilder_DoesNotThrow()
    {
        // Arrange
        var endpoint = new GetProductReviewsEndpoint();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockEndpointRouteBuilder = new Mock<IEndpointRouteBuilder>();

        mockEndpointRouteBuilder.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);

        mockEndpointRouteBuilder.Setup(x => x.DataSources).Returns([]);

        // Act & Assert
        Should.NotThrow(() => endpoint.MapEndpoint(mockEndpointRouteBuilder.Object));
    }
}
