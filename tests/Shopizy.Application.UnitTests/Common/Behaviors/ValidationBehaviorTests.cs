using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Shopizy.Application.Common.Behaviors;
using Shouldly;
using MediatR;

namespace Shopizy.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _mockValidator;
    private readonly Mock<RequestHandlerDelegate<ErrorOr<TestResponse>>> _mockNext;

    public ValidationBehaviorTests()
    {
        _mockValidator = new Mock<IValidator<TestRequest>>();
        _mockNext = new Mock<RequestHandlerDelegate<ErrorOr<TestResponse>>>();
    }

    [Fact]
    public async Task Handle_WhenNoValidator_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, ErrorOr<TestResponse>>(null);
        var request = new TestRequest();
        var expectedResponse = (ErrorOr<TestResponse>)new TestResponse();
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(request, _mockNext.Object, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockNext.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationSucceeds_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, ErrorOr<TestResponse>>(_mockValidator.Object);
        var request = new TestRequest();
        var expectedResponse = (ErrorOr<TestResponse>)new TestResponse();
        
        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(request, _mockNext.Object, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockNext.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, ErrorOr<TestResponse>>(_mockValidator.Object);
        var request = new TestRequest();
        var validationFailures = new List<ValidationFailure>
        {
            new("Prop", "Error")
        };
        
        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await behavior.Handle(request, _mockNext.Object, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(1);
        result.FirstError.Type.ShouldBe(ErrorType.Validation);
        result.FirstError.Code.ShouldBe("Prop");
        result.FirstError.Description.ShouldBe("Error");
        _mockNext.Verify(x => x(), Times.Never);
    }

    public class TestRequest : IRequest<ErrorOr<TestResponse>> { }
    public class TestResponse { }
}
