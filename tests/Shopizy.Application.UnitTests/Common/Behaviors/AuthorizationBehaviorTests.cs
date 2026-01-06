using ErrorOr;
using MediatR;
using Moq;
using Shopizy.Application.Common.Behaviors;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Security.Request;
using Shouldly;

namespace Shopizy.Application.UnitTests.Common.Behaviors;

public class AuthorizationBehaviorTests
{
    private readonly Mock<IAuthorizationService> _mockAuthorizationService;
    private readonly Mock<RequestHandlerDelegate<ErrorOr<TestResponse>>> _mockNext;

    public AuthorizationBehaviorTests()
    {
        _mockAuthorizationService = new Mock<IAuthorizationService>();
        _mockNext = new Mock<RequestHandlerDelegate<ErrorOr<TestResponse>>>();
    }

    [Fact]
    public async Task Handle_WhenNoAuthorizeAttribute_ShouldCallNext()
    {
        // Arrange
        var behavior = new AuthorizationBehavior<TestRequestWithoutAuthorize, ErrorOr<TestResponse>>(_mockAuthorizationService.Object);
        var request = new TestRequestWithoutAuthorize();
        var expectedResponse = (ErrorOr<TestResponse>)new TestResponse();
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(request, _mockNext.Object, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockNext.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAuthorizationSucceeds_ShouldCallNext()
    {
        // Arrange
        var behavior = new AuthorizationBehavior<TestRequestWithAuthorize, ErrorOr<TestResponse>>(_mockAuthorizationService.Object);
        var request = new TestRequestWithAuthorize();
        var expectedResponse = (ErrorOr<TestResponse>)new TestResponse();

        _mockAuthorizationService.Setup(s => s.AuthorizeCurrentUser(
            It.IsAny<List<string>>(),
            It.IsAny<List<string>>(),
            It.IsAny<List<string>>()))
            .Returns(Result.Success);
            
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(request, _mockNext.Object, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockNext.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAuthorizationFails_ShouldReturnError()
    {
        // Arrange
        var behavior = new AuthorizationBehavior<TestRequestWithAuthorize, ErrorOr<TestResponse>>(_mockAuthorizationService.Object);
        var request = new TestRequestWithAuthorize();
        var error = Error.Unauthorized();

        _mockAuthorizationService.Setup(s => s.AuthorizeCurrentUser(
            It.IsAny<List<string>>(),
            It.IsAny<List<string>>(),
            It.IsAny<List<string>>()))
            .Returns(error);

        // Act
        var result = await behavior.Handle(request, _mockNext.Object, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
        _mockNext.Verify(x => x(), Times.Never);
    }

    public class TestResponse { }

    public class TestRequestWithoutAuthorize : IAuthorizeableRequest<ErrorOr<TestResponse>> 
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
    }

    [Authorize(Permissions = "TestPermission", Roles = "TestRole", Policies = "TestPolicy")]
    public class TestRequestWithAuthorize : IAuthorizeableRequest<ErrorOr<TestResponse>> 
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
    }
}
