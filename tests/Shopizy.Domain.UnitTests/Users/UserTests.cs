using Xunit;
using Shouldly;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.Enums;
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
        var user = User.Create(firstName, lastName, email, password, UserRole.Customer, permissions);

        // Assert
        user.ShouldNotBeNull();
        user.FirstName.ShouldBe(firstName);
        user.LastName.ShouldBe(lastName);
        user.Email.ShouldBe(email);
        user.Password.ShouldBe(password);
        user.Role.ShouldBe(UserRole.Customer);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateUserDetailsAndAddress()
    {
        // Arrange
        var user = User.Create("Old", "User", "old@test.com", "hash", UserRole.Customer, []);
        var newFirst = "NewFirst";
        var newLast = "NewLast";

        // Act
        user.UpdateUserName(newFirst, newLast);
        user.UpdateAddress("Street", "City", "State", "Country", "12345");

        // Assert
        user.FirstName.ShouldBe(newFirst);
        user.LastName.ShouldBe(newLast);
        user.Address.ShouldNotBeNull();
        user.Address!.Street.ShouldBe("Street");
    }

    [Fact]
    public void UpdatePassword_ShouldUpdatePassword()
    {
        // Arrange
        var user = User.Create("U", "U", "e@e.com", "old", UserRole.Customer, new List<PermissionId>());
        var newPassword = "new_hashed_password";

        // Act
        user.UpdatePassword(newPassword);

        // Assert
        user.Password.ShouldBe(newPassword);
    }

    [Fact]
    public void UpdateRole_ShouldUpdateRole()
    {
        // Arrange
        var user = User.Create("U", "U", "e@e.com", "pass", UserRole.Customer, []);

        // Act
        user.UpdateRole(UserRole.Admin);

        // Assert
        user.Role.ShouldBe(UserRole.Admin);
    }
}
