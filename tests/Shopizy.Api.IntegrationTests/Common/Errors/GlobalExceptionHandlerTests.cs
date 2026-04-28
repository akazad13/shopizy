using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Shopizy.Api.Common.Errors;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Common.Errors;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _sut;

    public GlobalExceptionHandlerTests()
    {
        _sut = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
    }

    [Fact]
    public async Task TryHandleAsync_WhenGenericException_ShouldReturn500WithProblemDetails()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Internal error details that should NOT be exposed");

        // Act
        var handled = await _sut.TryHandleAsync(
            httpContext,
            exception,
            TestContext.Current.CancellationToken
        );

        // Assert
        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);

        var problemDetails = await ReadProblemDetailsAsync(httpContext);
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        problemDetails.Title.ShouldBe("An unexpected error occurred.");
    }

    [Fact]
    public async Task TryHandleAsync_WhenDbUpdateConcurrencyException_ShouldReturn409()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new DbUpdateConcurrencyException("Concurrency conflict");

        // Act
        var handled = await _sut.TryHandleAsync(
            httpContext,
            exception,
            TestContext.Current.CancellationToken
        );

        // Assert
        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status409Conflict);

        var problemDetails = await ReadProblemDetailsAsync(httpContext);
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(StatusCodes.Status409Conflict);
        problemDetails.Title.ShouldBe("Conflict");
    }

    [Fact]
    public async Task TryHandleAsync_WhenOperationCanceledException_ShouldReturn499()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new OperationCanceledException("Client closed request");

        // Act
        var handled = await _sut.TryHandleAsync(
            httpContext,
            exception,
            TestContext.Current.CancellationToken
        );

        // Assert
        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(499);

        var problemDetails = await ReadProblemDetailsAsync(httpContext);
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(499);
        problemDetails.Title.ShouldBe("Client Closed Request");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldIncludeTraceIdInResponse()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var expectedTraceId = httpContext.TraceIdentifier;
        var exception = new Exception("Some error");

        // Act
        await _sut.TryHandleAsync(httpContext, exception, TestContext.Current.CancellationToken);

        // Assert
        var responseBody = await ReadResponseBodyAsync(httpContext);
        var doc = JsonDocument.Parse(responseBody);
        doc.RootElement.TryGetProperty("traceId", out var traceIdElement).ShouldBeTrue();
        traceIdElement.GetString().ShouldBe(expectedTraceId);
    }

    [Fact]
    public async Task TryHandleAsync_WhenGenericException_ShouldNotExposeExceptionMessage()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var sensitiveMessage =
            "Sensitive internal error: connection string = Server=prod;Password=secret";
        var exception = new Exception(sensitiveMessage);

        // Act
        await _sut.TryHandleAsync(httpContext, exception, TestContext.Current.CancellationToken);

        // Assert
        var responseBody = await ReadResponseBodyAsync(httpContext);
        responseBody.ShouldNotContain(sensitiveMessage);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<ProblemDetails?> ReadProblemDetailsAsync(
        DefaultHttpContext httpContext
    )
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        return JsonSerializer.Deserialize<ProblemDetails>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }

    private static async Task<string> ReadResponseBodyAsync(DefaultHttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
    }
}
