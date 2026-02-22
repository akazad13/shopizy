using ErrorOr;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints;

public static class CustomResults
{
    public static IResult Problem(IList<Error> errors)
    {
        if (errors == null || errors.Count is 0)
        {
            return Results.BadRequest();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    private static IResult Problem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Results.Json(ErrorResult.Failure([error.Description]), statusCode: statusCode);
    }

    private static IResult ValidationProblem(IList<Error> errors)
    {
        return Results.Json(
            ErrorResult.Failure(errors.Select(error => error.Description).ToList()),
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}
