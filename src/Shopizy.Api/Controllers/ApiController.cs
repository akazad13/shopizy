using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected IActionResult Problem(IList<Error> errors)
    {
        if (errors == null || errors.Count is 0)
        {
            return BadRequest(new ModelStateDictionary());
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    private ObjectResult Problem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };

        return StatusCode(statusCode, ErrorResult.Failure([error.Description]));
    }

    private ObjectResult ValidationProblem(IList<Error> errors)
    {
        return StatusCode(
            StatusCodes.Status400BadRequest,
            ErrorResult.Failure(errors.Select(error => error.Description))
        );
    }
}
