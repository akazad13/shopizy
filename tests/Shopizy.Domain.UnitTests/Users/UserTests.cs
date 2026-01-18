using Xunit;
using Shouldly;
using Shopizy.Domain.Users;
using Shopizy.Domain.Permissions.ValueObjects;

namespace Shopizy.Domain.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var firstName = "First";
        var lastName = "Last";
        var email = "test@example.com";
        var password = "hashed_password";
        var permissions = new List<PermissionId>();

        // Act
        var user = User.Create(firstName, lastName, email, password, permissions);

        // Assert
        user.ShouldNotBeNull();
        user.FirstName.ShouldBe(firstName);
        user.LastName.ShouldBe(lastName);
        user.Email.ShouldBe(email);
        user.Password.ShouldBe(password);
        user.CreatedOn.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateUserDetailsAndAddress()
    {
        // Arrange
        var user = User.Create("Old", "User", "old@test.com", "hash", new List<PermissionId>());
        var newFirst = "NewFirst";
        var newLast = "NewLast";
        var newEmail = "new@test.com";
        var newPhone = "123456789";

        // Act
        user.Update(newFirst, newLast, newEmail, newPhone, "Street", "City", "State", "Country", "12345");

        // Assert
        user.FirstName.ShouldBe(newFirst);
        user.LastName.ShouldBe(newLast);
        user.Email.ShouldBe(newEmail);
        user.Phone.ShouldBe(newPhone);
        user.Address.ShouldNotBeNull();
        user.Address!.Street.ShouldBe("Street");
        user.ModifiedOn.ShouldNotBeNull();
    }

    [Fact]
    public void UpdatePassword_ShouldUpdatePassword()
    {
        // Arrange
        var user = User.Create("U", "U", "e@e.com", "old", new List<PermissionId>());
        var newPassword = "new_hashed_password";

        // Act
        user.UpdatePassword(newPassword);

        // Assert
        user.Password.ShouldBe(newPassword);
        user.ModifiedOn.ShouldNotBeNull();
    }
}
