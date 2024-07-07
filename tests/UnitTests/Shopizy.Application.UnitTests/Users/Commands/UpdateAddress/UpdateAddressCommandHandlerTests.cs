using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdateAddress;

public class UpdateAddressCommandHandlerTests
{
    private readonly UpdateAddressCommandHandler _handler;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public UpdateAddressCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new UpdateAddressCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task UpdateAddress_WhenUserIsFound_ShouldUpdateReturnSuccess()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user = UserFactory.UpdateAddress(user);
        var query = UpdateAddressCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(u => u.Update(user));

        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(query.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        result.IsError.Should().BeFalse();
    }
}
