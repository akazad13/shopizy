using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Moq;
using Shouldly;
using Shopizy.Api.Endpoints.ProductReviews;
using Xunit;


namespace Shopizy.Api.Endpoints.ProductReviews.UnitTests;

/// <summary>
/// Unit tests for DeleteProductReviewEndpoint.
/// </summary>
public class DeleteProductReviewEndpointTests
{
    /// <summary>
    /// Tests that MapEndpoint executes successfully with a valid IEndpointRouteBuilder.
    /// This test verifies the endpoint registration completes without throwing exceptions.
    /// Note: Due to the nature of ASP.NET Core endpoint registration using extension methods
    /// that cannot be mocked, comprehensive testing of endpoint configuration requires
    /// integration testing with a test server.
    /// </summary>
    [Fact]
    public void MapEndpoint_WithValidEndpointRouteBuilder_ExecutesSuccessfully()
    {
        // Arrange
        var mockEndpointRouteBuilder = new Mock<IEndpointRouteBuilder>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockEndpointDataSource = new Mock<EndpointDataSource>();

        mockEndpointRouteBuilder.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockEndpointRouteBuilder.Setup(x => x.CreateApplicationBuilder()).Returns(new Mock<IApplicationBuilder>().Object);
        mockEndpointRouteBuilder.Setup(x => x.DataSources).Returns(new List<EndpointDataSource> { mockEndpointDataSource.Object });

        var endpoint = new DeleteProductReviewEndpoint();

        // Act
        var exception = Record.Exception(() => endpoint.MapEndpoint(mockEndpointRouteBuilder.Object));

        // Assert
        exception.ShouldBeNull();
    }

    /// <summary>
    /// Tests that MapEndpoint throws ArgumentNullException when app parameter is null.
    /// This verifies that the method does not accept null endpoint route builders.
    /// </summary>
    [Fact]
    public void MapEndpoint_WithNullEndpointRouteBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        var endpoint = new DeleteProductReviewEndpoint();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => endpoint.MapEndpoint(null!));
    }
}