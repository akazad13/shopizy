using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Auth.Commands.Register;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.Users;

namespace Shopizy.Application.UnitTests.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordManager> _mockPasswordManager;
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordManager = new Mock<IPasswordManager>();
        _mockCartRepository = new Mock<ICartRepository>();
        _handler = new RegisterCommandHandler(
            _mockUserRepository.Object,
            _mockPasswordManager.Object,
            _mockCartRepository.Object
        );
    }

    [Fact]
    public async Task ShouldReturnDuplicateEmailErrorWhenUserWithSameEmailAlreadyExists()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "test@test.com", "password123");

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync(User.Create("Existing", "User", command.Email, "hashedPassword", []));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CustomErrors.User.DuplicateEmail);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShouldCreateNewUserWithHashedPasswordWhenValidInputIsProvided()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "test@test.com", "password123");
        var hashedPassword = "hashedPassword123";
        var expectedPermissionIds = new List<PermissionId>
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

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        _mockPasswordManager
            .Setup(pm => pm.CreateHashString(command.Password, 10000))
            .Returns(hashedPassword);

        _mockUserRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(command.Email), Times.Once);
        _mockPasswordManager.Verify(pm => pm.CreateHashString(command.Password, 10000), Times.Once);
        _mockUserRepository.Verify(
            r =>
                r.AddAsync(
                    It.Is<User>(u =>
                        u.FirstName == command.FirstName
                        && u.LastName == command.LastName
                        && u.Email == command.Email
                        && u.Password == hashedPassword
                        && u.PermissionIds.Select(p => p.Value)
                            .SequenceEqual(expectedPermissionIds.Select(p => p.Value))
                    )
                ),
            Times.Once
        );
        _mockUserRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnUserNotCreatedErrorWhenRepositoryCommitFails()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "1234567890", "password123");
        var hashedPassword = "hashedPassword123";

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        _mockPasswordManager
            .Setup(pm => pm.CreateHashString(command.Password, 10000))
            .Returns(hashedPassword);

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        _mockUserRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CustomErrors.User.UserNotCreated);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(command.Email), Times.Once);
        _mockPasswordManager.Verify(pm => pm.CreateHashString(command.Password, 10000), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Never);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShouldAssignAllDefaultPermissionsWhenCreatingNewUser()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "1234567890", "password123");
        var hashedPassword = "hashedPassword123";

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        _mockPasswordManager
            .Setup(pm => pm.CreateHashString(command.Password, 10000))
            .Returns(hashedPassword);

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        _mockUserRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();

        _mockUserRepository.Verify(
            r =>
                r.AddAsync(
                    It.Is<User>(u =>
                        u.PermissionIds.Count == 12
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("249E733D-5BDC-49C3-91CA-06AE25A9C897"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("D6C2E3C6-314B-4F2E-A407-34139B145771"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("9601BA5E-EB54-4487-BFE0-563462D3CC25"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("0374E597-604E-4146-8F40-8C994D26C290"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("ACD9D507-AC45-4CD2-B0F4-91126C71319A"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("2A19090A-B3F3-4B30-9CED-934EE0503D26"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("4B88CB16-0228-4669-BA7F-B75F42A3B7AF"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("5E2A486B-D9A0-4F83-8FF2-C56EF97CE485"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("DD25381D-063C-4A3A-9539-DEEC640919A4"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("20082930-3857-4B34-80D0-E256B9B585D8"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("0C65A58A-D472-4D5D-848E-EAC46F988F5D"))
                        )
                        && u.PermissionIds.Contains(
                            PermissionId.Create(new Guid("C920A577-1669-4167-B056-5E0A03329C55"))
                        )
                    )
                ),
            Times.Once
        );
    }

    [Theory]
    [InlineData("", "Doe")]
    [InlineData("  ", "Doe")]
    [InlineData("John", "")]
    [InlineData("John", "   ")]
    [InlineData("", "")]
    [InlineData("  ", "   ")]
    public async Task ShouldReturnValidationErrorWhenFirstNameOrLastNameIsEmptyOrWhitespace(
        string firstName,
        string lastName
    )
    {
        // Arrange
        var command = new RegisterCommand(firstName, lastName, "1234567890", "password123");

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CustomErrors.User.InvalidName);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Never);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _mockPasswordManager.Verify(
            p => p.CreateHashString(It.IsAny<string>(), 10000),
            Times.Never
        );
    }

    // [Theory]
    // [InlineData("123")]
    // [InlineData("12345678901234")]
    // [InlineData("abcdefghij")]
    // [InlineData("123-456-7890")]
    // public async Task ShouldReturnValidationErrorWhenPhoneNumberFormatIsInvalid(
    //     string invalidPhone
    // )
    // {
    //     // Arrange
    //     var command = new RegisterCommand("John", "Doe", invalidPhone, "password123");

    //     _mockUserRepository
    //         .Setup(r => r.GetUserByPhoneAsync(invalidPhone))
    //         .ReturnsAsync((User?)null);

    //     // Act
    //     var result = await _handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsError.Should().BeTrue();
    //     result.Errors.Should().Contain(CustomErrors.User.InvalidPhoneFormat);
    //     _mockUserRepository.Verify(r => r.GetUserByPhoneAsync(invalidPhone), Times.Never);
    //     _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    //     _mockUserRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Never);
    //     _mockPasswordManager.Verify(
    //         p => p.CreateHashString(It.IsAny<string>(), 10000),
    //         Times.Never
    //     );
    // }

    [Fact]
    public async Task ShouldEnsurePasswordIsHashedBeforeStoringInDatabase()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "1234567890", "password123");
        var hashedPassword = "hashedPassword123";

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);
        _mockPasswordManager
            .Setup(pm => pm.CreateHashString(command.Password, 10000))
            .Returns(hashedPassword);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        _mockPasswordManager.Verify(pm => pm.CreateHashString(command.Password, 10000), Times.Once);
        _mockUserRepository.Verify(
            r => r.AddAsync(It.Is<User>(u => u.Password == hashedPassword)),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldHandleConcurrentRegistrationAttemptsForSameEmail()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "test@test.com", "password123");
        var hashedPassword = "hashedPassword123";

        _mockUserRepository
            .SetupSequence(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((User?)null)
            .ReturnsAsync(User.Create("Existing", "User", command.Email, "existingHash", []));

        _mockPasswordManager
            .Setup(pm => pm.CreateHashString(command.Password, 1000))
            .Returns(hashedPassword);

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var task1 = _handler.Handle(command, CancellationToken.None);
        var task2 = _handler.Handle(command, CancellationToken.None);

        var results = await Task.WhenAll(task1, task2);

        // Assert
        results.Should().HaveCount(2);
        results[0].IsError.Should().BeFalse();
        results[0].Value.Should().BeOfType<Success>();
        results[1].IsError.Should().BeTrue();
        results[1].Errors.Should().Contain(CustomErrors.User.DuplicateEmail);

        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(command.Email), Times.Exactly(2));
        _mockPasswordManager.Verify(pm => pm.CreateHashString(command.Password, 10000), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Fact]
    // public async Task ShouldRespectCancellationToken()
    // {
    //     // Arrange
    //     var command = new RegisterCommand("John", "Doe", "1234567890", "password123");
    //     var cancellationTokenSource = new CancellationTokenSource();
    //     var cancellationToken = cancellationTokenSource.Token;

    //     _mockUserRepository
    //         .Setup(r => r.GetUserByPhoneAsync(command.Phone))
    //         .ReturnsAsync((User?)null);

    //     _mockPasswordManager
    //         .Setup(pm => pm.CreateHashString(command.Password, 10000))
    //         .Returns("hashedPassword");

    //     _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

    //     _mockUserRepository.Setup(r => r.Commit(cancellationToken)).ReturnsAsync(1);

    //     // Act
    //     var task = _handler.Handle(command, cancellationToken);
    //     await cancellationTokenSource.CancelAsync();
    //     cancellationTokenSource.Dispose();

    //     // Assert
    //     await Assert.ThrowsAsync<OperationCanceledException>(() => task);

    //     _mockUserRepository.Verify(r => r.GetUserByPhoneAsync(command.Phone), Times.Once);
    //     _mockPasswordManager.Verify(pm => pm.CreateHashString(command.Password, 10000), Times.Once);
    //     _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    //     _mockUserRepository.Verify(r => r.Commit(cancellationToken), Times.Once);
    // }

    [Fact]
    public async Task ShouldHandleExtremelyLongInputValuesForFirstNameLastNameAndEmail()
    {
        // Arrange
        var longFirstName = new string('A', 1000);
        var longLastName = new string('B', 1000);
        var longEmail = new string('a', 500);
        longEmail += "@test.com";
        var command = new RegisterCommand(longFirstName, longLastName, longEmail, "password123");
        var hashedPassword = "hashedPassword123";

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);
        _mockPasswordManager
            .Setup(pm => pm.CreateHashString(command.Password, 10000))
            .Returns(hashedPassword);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(command.Email), Times.Once);
        _mockPasswordManager.Verify(pm => pm.CreateHashString(command.Password, 10000), Times.Once);
        _mockUserRepository.Verify(
            r =>
                r.AddAsync(
                    It.Is<User>(u =>
                        u.FirstName == longFirstName
                        && u.LastName == longLastName
                        && u.Email == longEmail
                        && u.Password == hashedPassword
                        && u.PermissionIds.Count == 12
                    )
                ),
            Times.Once
        );
        _mockUserRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
