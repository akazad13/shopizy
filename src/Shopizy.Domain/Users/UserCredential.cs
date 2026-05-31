namespace Shopizy.Domain.Users;

/// <summary>
/// Owned type encapsulating all credential-related state for a <see cref="User"/>.
/// Stored in the same Users table row via EF owned-entity mapping.
/// </summary>
public sealed class UserCredential
{
    /// <summary>Gets the user's hashed password.</summary>
    public string? Password { get; private set; }

    /// <summary>Gets the password reset token.</summary>
    public string? PasswordResetToken { get; private set; }

    /// <summary>Gets the password reset token expiry.</summary>
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    /// <summary>Gets the two-factor authentication secret.</summary>
    public string? TwoFactorSecret { get; private set; }

    /// <summary>Gets whether two-factor authentication is enabled.</summary>
    public bool IsTwoFactorEnabled { get; private set; }

    internal UserCredential(string? password)
    {
        Password = password;
    }

    // Required by EF Core for owned-entity materialisation
    private UserCredential() { }

    /// <summary>Updates the hashed password.</summary>
    /// <param name="password"></param>
    public void UpdatePassword(string password) => Password = password;

    /// <summary>Sets the password reset token and its expiry.</summary>
    /// <param name="token"></param>
    /// <param name="expiry"></param>
    public void SetPasswordResetToken(string token, DateTime expiry)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = expiry;
    }

    /// <summary>Returns true when <paramref name="token"/> matches and has not expired.</summary>
    /// <param name="token"></param>
    public bool IsPasswordResetTokenValid(string token) =>
        PasswordResetToken == token && PasswordResetTokenExpiry > DateTime.UtcNow;

    /// <summary>Clears the password reset token after use.</summary>
    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }

    /// <summary>Generates a new TOTP secret and marks 2FA as pending confirmation.</summary>
    public string EnableTwoFactor()
    {
        TwoFactorSecret = GenerateBase32Secret();
        IsTwoFactorEnabled = false;
        return TwoFactorSecret;
    }

    /// <summary>Marks 2FA as fully enabled after code verification.</summary>
    public void ConfirmTwoFactor() => IsTwoFactorEnabled = true;

    /// <summary>Removes the TOTP secret and disables 2FA.</summary>
    public void DisableTwoFactor()
    {
        TwoFactorSecret = null;
        IsTwoFactorEnabled = false;
    }

    private static string GenerateBase32Secret()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(20);
        return Base32Encode(bytes);
    }

    private static string Base32Encode(byte[] data)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < data.Length; i += 5)
        {
            int byteCount = Math.Min(5, data.Length - i);
            ulong buffer = 0;
            for (int j = 0; j < byteCount; j++)
                buffer |= ((ulong)data[i + j]) << (8 * (4 - j));
            for (int j = 7; j >= 0 - (5 - byteCount) * 2; j--)
                result.Append(alphabet[(int)((buffer >> (j * 5)) & 0x1F)]);
        }
        return result.ToString();
    }
}
