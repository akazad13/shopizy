namespace Shopizy.Contracts.Common;

/// <summary>
/// Represents an error response.
/// </summary>
public class ErrorResult
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the list of error details.
    /// </summary>
    public string[] Errors { get; set; }

    internal ErrorResult(IEnumerable<string> errors)
    {
        Message = "Error occured";
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Creates a failure result from a list of errors.
    /// </summary>
    /// <param name="errors">The list of error messages.</param>
    /// <returns>An ErrorResult instance.</returns>
    public static ErrorResult Failure(IEnumerable<string> errors) => new(errors);
}
