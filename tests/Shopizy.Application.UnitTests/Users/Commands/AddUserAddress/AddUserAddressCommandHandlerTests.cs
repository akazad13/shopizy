using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.AddUserAddress;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.Entities;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.AddUserAddress;

public class AddUserAddressCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly AddUserAddressCommandHandler _handler;

    public AddUserAddressCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new AddUserAddressCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = new AddUserAddressCommand(
            Guid.NewGuid(),
            "123 Main St",
            "Anytown",
            "NY",
            "USA",
            "10001",
            true
        );

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((Shopizy.Domain.Users.User?)null);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.User.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldAddAddressAndReturnUserAddress()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = new AddUserAddressCommand(
            user.Id.Value,
            "123 Main St",
            "Anytown",
            "NY",
            "USA",
            "10001",
            true
        );

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Street.ShouldBe("123 Main St");
        result.Value.City.ShouldBe("Anytown");
        result.Value.IsDefault.ShouldBeTrue();
        user.Addresses.Count.ShouldBe(1);
    }
}
