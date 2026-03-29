namespace Shopizy.Contracts.Authentication;

public record ResetPasswordRequest(string Token, string NewPassword);
