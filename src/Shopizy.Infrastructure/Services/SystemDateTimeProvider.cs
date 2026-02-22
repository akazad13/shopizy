using System;
using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Infrastructure.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
