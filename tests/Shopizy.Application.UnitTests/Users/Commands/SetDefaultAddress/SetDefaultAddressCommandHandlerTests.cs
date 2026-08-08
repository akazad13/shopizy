using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.SetDefaultAddress;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.SetDefaultAddress;

public class SetDefaultAddressCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly SetDefaultAddressCommandHandler _handler;

    public SetDefaultAddressCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new SetDefaultAddressCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = new SetDefaultAddressCommand(Guid.NewGuid(), Guid.NewGuid());

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
        var command = new SetDefaultAddressCommand(user.Id.Value, Guid.NewGuid());

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.UserAddress.AddressNotFound);
    }

    [Fact]
    public async Task Handle_WhenAddressExists_ShouldSetDefaultAndReturnSuccess()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var addr1 = user.AddAddress("Street 1", "City", "State", "Country", "12345", true);
        var addr2 = user.AddAddress("Street 2", "City", "State", "Country", "12345", false);
        var command = new SetDefaultAddressCommand(user.Id.Value, addr2.Id.Value);

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
        addr1.IsDefault.ShouldBeFalse();
        addr2.IsDefault.ShouldBeTrue();
    }
}
