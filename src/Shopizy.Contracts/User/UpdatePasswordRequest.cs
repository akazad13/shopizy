namespace Shopizy.Contracts.User;

public class UpdatePasswordRequest
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
