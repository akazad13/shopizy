using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.UpdateUserAddress;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdateUserAddress;

public class UpdateUserAddressCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UpdateUserAddressCommandHandler _handler;

    public UpdateUserAddressCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new UpdateUserAddressCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = new UpdateUserAddressCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New St",
            "City",
            "State",
            "Country",
            "99999"
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
    public async Task Handle_WhenAddressNotFound_ShouldReturnAddressNotFoundError()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = new UpdateUserAddressCommand(
            user.Id.Value,
            Guid.NewGuid(),
            "New St",
            "City",
            "State",
            "Country",
            "99999"
        );

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.UserAddress.AddressNotFound);
    }

    [Fact]
    public async Task Handle_WhenAddressExists_ShouldUpdateAndReturnUserAddress()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var address = user.AddAddress(
            "Old St",
            "Old City",
            "Old State",
            "Old Country",
            "00000",
            false
        );
        var command = new UpdateUserAddressCommand(
            user.Id.Value,
            address.Id.Value,
            "New St",
            "New City",
            "New State",
            "New Country",
            "99999"
        );

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Street.ShouldBe("New St");
        result.Value.City.ShouldBe("New City");
        result.Value.ZipCode.ShouldBe("99999");
    }
}
