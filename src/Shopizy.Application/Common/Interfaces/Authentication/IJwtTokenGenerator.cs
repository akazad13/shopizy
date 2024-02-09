using Shopizy.Domain.Users.ValueObject;

namespace Shopizy.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        UserId userId,
        string firstName,
        string LastName,
        List<string> roles,
        List<string> Permissions
    );
}
