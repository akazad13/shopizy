namespace Shopizy.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Guid userId,
        string firstName,
        string LastName,
        List<string> roles,
        List<string> Permissions
    );
}
