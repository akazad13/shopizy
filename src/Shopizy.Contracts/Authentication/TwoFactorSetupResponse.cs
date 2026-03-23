namespace Shopizy.Contracts.Authentication;

public record TwoFactorSetupResponse(string Secret, string QrCodeUri);
