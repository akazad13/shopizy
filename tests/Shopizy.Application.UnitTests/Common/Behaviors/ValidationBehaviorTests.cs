using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Shopizy.SharedKernel.Application.Behaviors;
using Shopizy.SharedKernel.Application.Messaging;
using Shouldly;

namespace Shopizy.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<TestCommand>> _mockValidator;
    private readonly Mock<ICommandHandler<TestCommand, ErrorOr<TestResponse>>> _mockHandler;

    public ValidationBehaviorTests()
    {
        _mockValidator = new Mock<IValidator<TestCommand>>();
        _mockHandler = new Mock<ICommandHandler<TestCommand, ErrorOr<TestResponse>>>();
    }

    [Fact]
    public async Task Handle_WhenNoValidator_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationCommandHandlerDecorator<TestCommand, ErrorOr<TestResponse>>(_mockHandler.Object, null);
        var request = new TestCommand();
        var expectedResponse = (ErrorOr<TestResponse>)new TestResponse();
        _mockHandler.Setup(x => x.Handle(request, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockHandler.Verify(x => x.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationSucceeds_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationCommandHandlerDecorator<TestCommand, ErrorOr<TestResponse>>(_mockHandler.Object, _mockValidator.Object);
        var request = new TestCommand();
        var expectedResponse = (ErrorOr<TestResponse>)new TestResponse();
        
        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockHandler.Setup(x => x.Handle(request, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await behavior.Handle(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResponse);
        _mockHandler.Verify(x => x.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationErrors()
    {
        // Arrange
        var behavior = new ValidationCommandHandlerDecorator<TestCommand, ErrorOr<TestResponse>>(_mockHandler.Object, _mockValidator.Object);
        var request = new TestCommand();
        var validationFailures = new List<ValidationFailure>
        {
            new("Prop", "Error")
        };
        
        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await behavior.Handle(request, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(1);
        result.FirstError.Type.ShouldBe(ErrorType.Validation);
        result.FirstError.Code.ShouldBe("Prop");
        result.FirstError.Description.ShouldBe("Error");
        _mockHandler.Verify(x => x.Handle(request, It.IsAny<CancellationToken>()), Times.Never);
    }

    public class TestCommand : ICommand<ErrorOr<TestResponse>> { }
    public class TestResponse { }
}
