using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Api.Endpoints;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Common;

public class CustomResultsTests
{
    [Fact]
    public void Problem_WhenNullOrEmptyErrors_ShouldReturnBadRequest()
    {
        var resultNull = CustomResults.Problem(null!);
        resultNull.ShouldNotBeNull();

        var resultEmpty = CustomResults.Problem([]);
        resultEmpty.ShouldNotBeNull();
    }

    [Fact]
    public void Problem_WhenAllValidationErrors_ShouldReturnBadRequest()
    {
        var errors = new List<Error>
        {
            Error.Validation("Val.1", "Validation 1"),
            Error.Validation("Val.2", "Validation 2"),
        };

        var result = CustomResults.Problem(errors);
        result.ShouldNotBeNull();
    }

    [Theory]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Unauthorized)]
    [InlineData(ErrorType.Forbidden)]
    [InlineData(ErrorType.Unexpected)]
    [InlineData((ErrorType)99)] // Default case
    public void Problem_WhenSingleError_ShouldMapToStatusCode(ErrorType type)
    {
        var error = Error.Custom((int)type, "Custom.Code", "Custom Description");
        var result = CustomResults.Problem([error]);
        result.ShouldNotBeNull();
    }
}
