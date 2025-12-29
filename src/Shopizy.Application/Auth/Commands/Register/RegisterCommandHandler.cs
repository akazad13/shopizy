using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Auth.Commands.Register;

/// <summary>
/// Handles the <see cref="RegisterCommand"/> to create a new user account.
/// </summary>
public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager,
    ICartRepository cartRepository
) : IRequestHandler<RegisterCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ICartRepository _cartRepository = cartRepository;

    /// <summary>
    /// Handles the user registration process including validation, password hashing, and cart creation.
    /// </summary>
    /// <param name="command">The registration command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A success result or an error.</returns>
    public async Task<ErrorOr<Success>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken
    )
    {
        // check if command.Email is valid email address
        if (string.IsNullOrEmpty(command.Email) || !IsValidEmail(command.Email))
        {
            return CustomErrors.User.InvalidEmailFormat;
        }

        if (await _userRepository.GetUserByEmailAsync(command.Email) is not null)
        {
            return CustomErrors.User.DuplicateEmail;
        }

        if (
            string.IsNullOrEmpty(command.FirstName?.Trim())
            || string.IsNullOrEmpty(command.LastName?.Trim())
        )
        {
            return CustomErrors.User.InvalidName;
        }

        var hashedPassword = _passwordManager.CreateHashString(command.Password);

        var permissionIds = new List<PermissionId>()
        {
            PermissionId.Create(new("249E733D-5BDC-49C3-91CA-06AE25A9C897")),
            PermissionId.Create(new("D6C2E3C6-314B-4F2E-A407-34139B145771")),
            PermissionId.Create(new("9601BA5E-EB54-4487-BFE0-563462D3CC25")),
            PermissionId.Create(new("0374E597-604E-4146-8F40-8C994D26C290")),
            PermissionId.Create(new("ACD9D507-AC45-4CD2-B0F4-91126C71319A")),
            PermissionId.Create(new("2A19090A-B3F3-4B30-9CED-934EE0503D26")),
            PermissionId.Create(new("4B88CB16-0228-4669-BA7F-B75F42A3B7AF")),
            PermissionId.Create(new("5E2A486B-D9A0-4F83-8FF2-C56EF97CE485")),
            PermissionId.Create(new("DD25381D-063C-4A3A-9539-DEEC640919A4")),
            PermissionId.Create(new("20082930-3857-4B34-80D0-E256B9B585D8")),
            PermissionId.Create(new("0C65A58A-D472-4D5D-848E-EAC46F988F5D")),
            PermissionId.Create(new("C920A577-1669-4167-B056-5E0A03329C55")),
        };

        var user = User.Create(
            command.FirstName,
            command.LastName,
            command.Email,
            hashedPassword,
            permissionIds
        );

        await _userRepository.AddAsync(user);

        await _userRepository.AddAsync(user);

        var cart = Cart.Create(user.Id);
        await _cartRepository.AddAsync(cart);

        return Result.Success;
    }

    // Helper method to validate email format
    static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
