using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.DeleteUserAddress;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.DeleteUserAddress;

public class DeleteUserAddressCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly DeleteUserAddressCommandHandler _handler;

    public DeleteUserAddressCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new DeleteUserAddressCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = new DeleteUserAddressCommand(Guid.NewGuid(), Guid.NewGuid());

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
        var command = new DeleteUserAddressCommand(user.Id.Value, Guid.NewGuid());

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.UserAddress.AddressNotFound);
    }

    [Fact]
    public async Task Handle_WhenAddressExists_ShouldRemoveAddressAndReturnDeleted()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var address = user.AddAddress("Street", "City", "State", "Country", "12345", false);
        var command = new DeleteUserAddressCommand(user.Id.Value, address.Id.Value);

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Deleted);
        user.Addresses.Count.ShouldBe(0);
    }
}
