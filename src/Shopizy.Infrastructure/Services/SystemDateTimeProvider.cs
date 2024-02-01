using Shopizy.Application.Common.Interfaces;

namespace Shopizy.Infrastructure.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
