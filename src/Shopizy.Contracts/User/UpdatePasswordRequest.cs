namespace Shopizy.Contracts.User;

/// <summary>
/// Represents a request to update a user's password.
/// </summary>
public class UpdatePasswordRequest
{
    /// <summary>
    /// Gets or sets the current password.
    /// </summary>
    public required string OldPassword { get; set; }

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    public required string NewPassword { get; set; }
}
