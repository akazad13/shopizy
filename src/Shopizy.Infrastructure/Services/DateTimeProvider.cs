using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="IDateTimeProvider"/> using system time.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}
