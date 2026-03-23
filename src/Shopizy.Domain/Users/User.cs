using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Users.Entities;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Users.Enums;
using ErrorOr;

namespace Shopizy.Domain.Users;

/// <summary>
/// Represents a user in the system.
/// </summary>
public sealed class User : AggregateRoot<UserId, Guid>, IAuditable
{
    private readonly List<OrderId> _orderIds = [];
    private readonly List<ProductReviewId> _productReviewIds = [];
    private readonly List<PermissionId> _permissionIds = [];
    private readonly List<UserAddress> _addresses = [];

    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the user's role.
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Gets the user's profile image URL.
    /// </summary>
    public string? ProfileImageUrl { get; private set;}

    /// <summary>
    /// Gets the user's phone number.
    /// </summary>
    public string Phone { get; private set;}

    /// <summary>
    /// Gets the user's hashed password.
    /// </summary>
    public string? Password { get; private set; }

    /// <summary>
    /// Gets the user's customer ID for payment processing.
    /// </summary>
    public string? CustomerId { get; private set; } = null;

    /// <summary>
    /// Gets the user's address.
    /// </summary>
    public Address? Address { get; private set; }

    /// <summary>
    /// Gets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedOn { get; }

    /// <summary>
    /// Gets the date and time when the user was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; }

    /// <summary>
    /// Gets the read-only list of order IDs associated with this user.
    /// </summary>
    public IReadOnlyList<OrderId> OrderIds => _orderIds.AsReadOnly();

    /// <summary>
    /// Gets the read-only list of product review IDs created by this user.
    /// </summary>
    public IReadOnlyList<ProductReviewId> ProductReviewIds => _productReviewIds.AsReadOnly();

    /// <summary>
    /// Gets the read-only list of permission IDs assigned to this user.
    /// </summary>
    public IReadOnlyList<PermissionId> PermissionIds => _permissionIds.AsReadOnly();

    /// <summary>
    /// Gets the read-only list of addresses associated with this user.
    /// </summary>
    public IReadOnlyList<UserAddress> Addresses => _addresses.AsReadOnly();

    /// <summary>
    /// Gets or sets the password reset token.
    /// </summary>
    public string? PasswordResetToken { get; private set; }

    /// <summary>
    /// Gets or sets the password reset token expiry.
    /// </summary>
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    /// <summary>
    /// Gets the two-factor authentication secret.
    /// </summary>
    public string? TwoFactorSecret { get; private set; }

    /// <summary>
    /// Gets whether two-factor authentication is enabled.
    /// </summary>
    public bool IsTwoFactorEnabled { get; private set; }

    /// <summary>
    /// Creates a new user instance.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's hashed password.</param>
    /// <param name="role">The user's role.</param>
    /// <param name="permissionIds">The list of permission IDs to assign to the user.</param>
    /// <returns>A new <see cref="User"/> instance.</returns>
    public static User Create(
        string firstName,
        string lastName,
        string email,
        string? password,
        UserRole role,
        IList<PermissionId> permissionIds
    )
    {
        var user = new User(UserId.CreateUnique(), firstName, lastName, email, password, role, permissionIds);
        user.AddDomainEvent(new Events.UserRegisteredDomainEvent(user));

        return user;
    }

    private User() { }

    private User(
        UserId userId,
        string firstName,
        string lastName,
        string email,
        string? password,
        UserRole role,
        IList<PermissionId> permissionIds
    )
        : base(userId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        Role = role;
        _permissionIds = [.. permissionIds];
    }

    /// <summary>
    /// Updates the user's address information.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state or province.</param>
    /// <param name="country">The country.</param>
    /// <param name="zipCode">The postal/ZIP code.</param>
    public void UpdateAddress(
        string street,
        string city,
        string state,
        string country,
        string zipCode
    )
    {
        Address = Address.CreateNew(street, city, state, country, zipCode);
    }

    /// <summary>
    /// Updates the user's password.
    /// </summary>
    /// <param name="password">The new hashed password.</param>
    public void UpdatePassword(string password)
    {
        Password = password;
    }

    /// <summary>
    /// Updates the user's name.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    public void UpdateUserName(
        string firstName,
        string lastName
    )
    {
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>
    /// Updates the user's phone.
    /// </summary>
    /// <param name="phone">The user's phone.</param>
    public void UpdatePhone(string phone)
    {
        Phone = phone;
    }

    /// <summary>
    /// Updates the user's profile picture.
    /// </summary>
    /// <param name="profileImageUrl">The user's profile picture.</param>
    public void UpdateProfileImageUrl(string profileImageUrl)
    {
        ProfileImageUrl = profileImageUrl;
    }

    /// <summary>
    /// Updates the user's customer ID for payment processing.
    /// </summary>
    /// <param name="customerId">The customer ID from the payment provider.</param>
    public void UpdateCustomerId(string customerId)
    {
        CustomerId = customerId;
    }

    /// <summary>
    /// Updates the user's role.
    /// </summary>
    /// <param name="role">The new role.</param>
    public void UpdateRole(UserRole role)
    {
        Role = role;
    }

    /// <summary>
    /// Updates the user's permissions.
    /// </summary>
    /// <param name="permissionIds">The new list of permission IDs.</param>
    public void UpdatePermissions(IList<PermissionId> permissionIds)
    {
        _permissionIds.Clear();
        _permissionIds.AddRange(permissionIds);
    }

    /// <summary>
    /// Adds a new address to the user's address book.
    /// </summary>
    public UserAddress AddAddress(
        string street,
        string city,
        string state,
        string country,
        string zipCode,
        bool isDefault
    )
    {
        if (isDefault)
        {
            foreach (var a in _addresses) a.SetDefault(false);
        }

        var address = UserAddress.Create(street, city, state, country, zipCode, isDefault);
        _addresses.Add(address);
        return address;
    }

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    public ErrorOr<UserAddress> UpdateAddress(
        UserAddressId addressId,
        string street,
        string city,
        string state,
        string country,
        string zipCode
    )
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null) return CustomErrors.UserAddress.AddressNotFound;
        address.Update(street, city, state, country, zipCode);
        return address;
    }

    /// <summary>
    /// Removes an address from the user's address book.
    /// </summary>
    public ErrorOr<Deleted> RemoveAddress(UserAddressId addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null) return CustomErrors.UserAddress.AddressNotFound;
        _addresses.Remove(address);
        return Result.Deleted;
    }

    /// <summary>
    /// Sets the default address for the user.
    /// </summary>
    public ErrorOr<Success> SetDefaultAddress(UserAddressId addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null) return CustomErrors.UserAddress.AddressNotFound;
        foreach (var a in _addresses) a.SetDefault(false);
        address.SetDefault(true);
        return Result.Success;
    }

    /// <summary>
    /// Sets the password reset token and expiry.
    /// </summary>
    public void SetPasswordResetToken(string token, DateTime expiry)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = expiry;
    }

    /// <summary>
    /// Validates whether the provided reset token is valid and not expired.
    /// </summary>
    public bool IsPasswordResetTokenValid(string token)
        => PasswordResetToken == token && PasswordResetTokenExpiry > DateTime.UtcNow;

    /// <summary>
    /// Clears the password reset token after use.
    /// </summary>
    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }

    /// <summary>
    /// Enables two-factor authentication by generating a secret.
    /// </summary>
    public string EnableTwoFactor()
    {
        TwoFactorSecret = GenerateBase32Secret();
        IsTwoFactorEnabled = false;
        return TwoFactorSecret;
    }

    /// <summary>
    /// Confirms two-factor authentication after successful code verification.
    /// </summary>
    public void ConfirmTwoFactor()
    {
        IsTwoFactorEnabled = true;
    }

    /// <summary>
    /// Disables two-factor authentication.
    /// </summary>
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
            for (int j = 0; j < byteCount; j++) buffer |= ((ulong)data[i + j]) << (8 * (4 - j));
            for (int j = 7; j >= 0 - (5 - byteCount) * 2; j--) result.Append(alphabet[(int)((buffer >> (j * 5)) & 0x1F)]);
        }
        return result.ToString();
    }
}
