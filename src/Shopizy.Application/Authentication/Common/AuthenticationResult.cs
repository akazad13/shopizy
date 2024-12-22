namespace Shopizy.Application.Authentication.Common;

public record AuthResult(Guid Id, string FirstName, string LastName, string Email, string Token);
