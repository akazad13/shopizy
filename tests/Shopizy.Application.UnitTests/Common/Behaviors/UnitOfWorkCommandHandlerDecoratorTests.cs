using ErrorOr;
using Moq;
using Shopizy.SharedKernel.Application.Behaviors;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Shouldly;

namespace Shopizy.Application.UnitTests.Common.Behaviors;

public class UnitOfWorkCommandHandlerDecoratorTests
{
    private readonly Mock<ICommandHandler<TestUowCommand, ErrorOr<TestUowResponse>>> _mockHandler;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UnitOfWorkCommandHandlerDecorator<TestUowCommand, ErrorOr<TestUowResponse>> _sut;

    public UnitOfWorkCommandHandlerDecoratorTests()
    {
        _mockHandler = new Mock<ICommandHandler<TestUowCommand, ErrorOr<TestUowResponse>>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _sut = new UnitOfWorkCommandHandlerDecorator<TestUowCommand, ErrorOr<TestUowResponse>>(
            _mockHandler.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WhenInnerHandlerReturnsSuccess_ShouldCallSaveChangesAsync()
    {
        // Arrange
        var command = new TestUowCommand();
        var expectedResponse = (ErrorOr<TestUowResponse>)new TestUowResponse();
        _mockHandler
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(expectedResponse.Value);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInnerHandlerReturnsError_ShouldNotCallSaveChangesAsync()
    {
        // Arrange
        var command = new TestUowCommand();
        var error = Error.Failure("Test.Error", "Something went wrong");
        var errorResponse = (ErrorOr<TestUowResponse>)error;
        _mockHandler
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(errorResponse);

        // Act
        var result = await _sut.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    public class TestUowCommand : ICommand<ErrorOr<TestUowResponse>> { }
    public class TestUowResponse { }
}
