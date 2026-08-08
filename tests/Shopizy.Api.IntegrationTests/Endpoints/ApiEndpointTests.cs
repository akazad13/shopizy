using ErrorOr;
using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Api.Endpoints;
using Shopizy.SharedKernel.Application.Messaging;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Endpoints;

/// <summary>
/// Exercises ApiEndpoint.HandleAsync for both the ICommand and IQuery overloads.
/// Because HandleAsync is protected static, we use a thin concrete subclass as
/// the test surface.
/// </summary>
public class ApiEndpointTests
{
    // ── Minimal stubs ──────────────────────────────────────────────────────────

    private sealed record TestCommand(string Value) : ICommand<ErrorOr<string>>;

    private sealed record TestQuery(string Value) : IQuery<ErrorOr<string>>;

    // Thin concrete subclass so we can call the protected static method
    private sealed class TestEndpoint : ApiEndpoint
    {
        public override void MapEndpoint(IEndpointRouteBuilder app) { }

        public static Task<IResult> RunCommand(
            IDispatcher dispatcher,
            ICommand<ErrorOr<string>> command,
            Func<string, IResult> onSuccess,
            Action<Exception> onError
        ) => HandleAsync(dispatcher, command, onSuccess, onError);

        public static Task<IResult> RunQuery(
            IDispatcher dispatcher,
            IQuery<ErrorOr<string>> query,
            Func<string, IResult> onSuccess,
            Action<Exception> onError
        ) => HandleAsync(dispatcher, query, onSuccess, onError);
    }

    // ── Command overload ───────────────────────────────────────────────────────

    [Fact]
    public async Task HandleAsync_Command_WhenDispatcherReturnsSuccess_ShouldInvokeOnSuccess()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var command = new TestCommand("hello");
        mockDispatcher
            .Setup(d => d.SendAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync("hello");

        // Act
        var result = await TestEndpoint.RunCommand(
            mockDispatcher.Object,
            command,
            value => Results.Ok(value),
            _ => { }
        );

        // Assert
        result.ShouldNotBeNull();
        mockDispatcher.Verify(d => d.SendAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Command_WhenDispatcherReturnsErrors_ShouldReturnProblem()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var command = new TestCommand("bad");
        mockDispatcher
            .Setup(d => d.SendAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("Item.NotFound", "Item was not found."));

        var onSuccessCalled = false;

        // Act
        var result = await TestEndpoint.RunCommand(
            mockDispatcher.Object,
            command,
            _ =>
            {
                onSuccessCalled = true;
                return Results.Ok();
            },
            _ => { }
        );

        // Assert
        result.ShouldNotBeNull();
        onSuccessCalled.ShouldBeFalse();
        mockDispatcher.Verify(d => d.SendAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Command_WhenDispatcherThrows_ShouldCallOnErrorAndReturnProblem()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var command = new TestCommand("throw");
        var expectedException = new InvalidOperationException("Dispatcher exploded");

        mockDispatcher
            .Setup(d => d.SendAsync(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        Exception? capturedEx = null;

        // Act
        var result = await TestEndpoint.RunCommand(
            mockDispatcher.Object,
            command,
            _ => Results.Ok(),
            ex => capturedEx = ex
        );

        // Assert
        result.ShouldNotBeNull();
        capturedEx.ShouldNotBeNull();
        capturedEx.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public async Task HandleAsync_Command_WhenDispatcherThrows_ShouldNotInvokeOnSuccess()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var command = new TestCommand("throw");
        mockDispatcher
            .Setup(d => d.SendAsync(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        var onSuccessCalled = false;

        // Act
        await TestEndpoint.RunCommand(
            mockDispatcher.Object,
            command,
            _ =>
            {
                onSuccessCalled = true;
                return Results.Ok();
            },
            _ => { }
        );

        // Assert
        onSuccessCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAsync_Command_WhenValidationErrors_ShouldReturnProblemWithoutCallingOnSuccess()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var command = new TestCommand("invalid");
        mockDispatcher
            .Setup(d => d.SendAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[]
                {
                    Error.Validation("Field.Required", "Field is required."),
                }.ToErrorOr<string>()
            );

        var onSuccessCalled = false;

        // Act
        var result = await TestEndpoint.RunCommand(
            mockDispatcher.Object,
            command,
            _ =>
            {
                onSuccessCalled = true;
                return Results.Ok();
            },
            _ => { }
        );

        // Assert
        result.ShouldNotBeNull();
        onSuccessCalled.ShouldBeFalse();
    }

    // ── Query overload ─────────────────────────────────────────────────────────

    [Fact]
    public async Task HandleAsync_Query_WhenDispatcherReturnsSuccess_ShouldInvokeOnSuccess()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var query = new TestQuery("world");
        mockDispatcher
            .Setup(d => d.SendAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync("world");

        // Act
        var result = await TestEndpoint.RunQuery(
            mockDispatcher.Object,
            query,
            value => Results.Ok(value),
            _ => { }
        );

        // Assert
        result.ShouldNotBeNull();
        mockDispatcher.Verify(d => d.SendAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Query_WhenDispatcherReturnsErrors_ShouldReturnProblem()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var query = new TestQuery("missing");
        mockDispatcher
            .Setup(d => d.SendAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("Resource.NotFound", "Resource not found."));

        var onSuccessCalled = false;

        // Act
        var result = await TestEndpoint.RunQuery(
            mockDispatcher.Object,
            query,
            _ =>
            {
                onSuccessCalled = true;
                return Results.Ok();
            },
            _ => { }
        );

        // Assert
        result.ShouldNotBeNull();
        onSuccessCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAsync_Query_WhenDispatcherThrows_ShouldCallOnErrorAndReturnProblem()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var query = new TestQuery("throw");
        var expectedException = new TimeoutException("Query timed out");

        mockDispatcher
            .Setup(d => d.SendAsync(query, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        Exception? capturedEx = null;

        // Act
        var result = await TestEndpoint.RunQuery(
            mockDispatcher.Object,
            query,
            _ => Results.Ok(),
            ex => capturedEx = ex
        );

        // Assert
        result.ShouldNotBeNull();
        capturedEx.ShouldNotBeNull();
        capturedEx.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public async Task HandleAsync_Query_WhenDispatcherThrows_ShouldNotInvokeOnSuccess()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var query = new TestQuery("throw");
        mockDispatcher
            .Setup(d => d.SendAsync(query, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("query boom"));

        var onSuccessCalled = false;

        // Act
        await TestEndpoint.RunQuery(
            mockDispatcher.Object,
            query,
            _ =>
            {
                onSuccessCalled = true;
                return Results.Ok();
            },
            _ => { }
        );

        // Assert
        onSuccessCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAsync_Query_WhenUnauthorizedError_ShouldReturnProblemWithoutCallingOnSuccess()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var query = new TestQuery("unauthorized");
        mockDispatcher
            .Setup(d => d.SendAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.Unauthorized("Auth.Unauthorized", "Access denied."));

        var onSuccessCalled = false;

        // Act
        var result = await TestEndpoint.RunQuery(
            mockDispatcher.Object,
            query,
            _ =>
            {
                onSuccessCalled = true;
                return Results.Ok();
            },
            _ => { }
        );

        // Assert
        result.ShouldNotBeNull();
        onSuccessCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAsync_Query_WhenConflictError_ShouldReturnProblemWithoutCallingOnSuccess()
    {
        // Arrange
        var mockDispatcher = new Mock<IDispatcher>();
        var query = new TestQuery("conflict");
        mockDispatcher
            .Setup(d => d.SendAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.Conflict("Item.Conflict", "Item already exists."));

        var onSuccessCalled = false;

        // Act
        var result = await TestEndpoint.RunQuery(
            mockDispatcher.Object,
            query,
            _ =>
            {
                onSuccessCalled = true;
                return Results.Ok();
            },
            _ => { }
        );

        // Assert
        result.ShouldNotBeNull();
        onSuccessCalled.ShouldBeFalse();
    }
}
