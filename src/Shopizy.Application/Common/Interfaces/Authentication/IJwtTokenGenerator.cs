using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserId userId, IEnumerable<string> Permissions);
}

