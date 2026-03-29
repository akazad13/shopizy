namespace Shopizy.Application.Users.Commands.EnableTwoFactor;

public record TwoFactorSetupDto(string Secret, string QrCodeUri);
