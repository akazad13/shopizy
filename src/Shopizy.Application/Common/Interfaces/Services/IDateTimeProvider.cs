namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// Provides an abstraction for retrieving the current date and time.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    DateTime UtcNow { get; }
}
