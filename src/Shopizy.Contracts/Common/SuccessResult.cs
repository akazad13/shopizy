namespace Shopizy.Contracts.Common;

/// <summary>
/// Represents a success response.
/// </summary>
public class SuccessResult
{
    /// <summary>
    /// Gets or sets the success message.
    /// </summary>
    public string Message { get; set; }

    internal SuccessResult(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates a success result with a message.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <returns>A SuccessResult instance.</returns>
    public static SuccessResult Success(string message) => new(message);
}
