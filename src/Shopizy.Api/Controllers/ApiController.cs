using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Controllers;

/// <summary>
/// Base controller for all API controllers, providing common error handling.
/// </summary>
[ApiController]
public class ApiController : ControllerBase
{
    /// <summary>
    /// Creates an action result based on a list of errors.
    /// </summary>
    /// <param name="errors">The list of errors.</param>
    /// <returns>An appropriate action result (BadRequest, ValidationProblem, or Problem).</returns>
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

    protected bool IsAuthorized(Guid userId)
    {
        var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserIdClaim == null) return false;
        
        return currentUserIdClaim == userId.ToString() || User.IsInRole("Admin");
    }

    private ObjectResult Problem(Error error)
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
