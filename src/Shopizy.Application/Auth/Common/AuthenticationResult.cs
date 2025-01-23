namespace Shopizy.Application.Auth.Common;

public record AuthResult(Guid Id, string FirstName, string LastName, string Email, string Token);
