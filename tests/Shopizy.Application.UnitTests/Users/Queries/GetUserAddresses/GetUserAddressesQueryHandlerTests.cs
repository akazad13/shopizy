using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Queries.GetUserAddresses;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Queries.GetUserAddresses;

public class GetUserAddressesQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUserAddressesQueryHandler _handler;

    public GetUserAddressesQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUserAddressesQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var query = new GetUserAddressesQuery(Guid.NewGuid());

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((Shopizy.Domain.Users.User?)null);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.User.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnListOfAddresses()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user.AddAddress("Street 1", "City 1", "State 1", "Country 1", "10001", true);
        user.AddAddress("Street 2", "City 2", "State 2", "Country 2", "10002", false);
        var query = new GetUserAddressesQuery(user.Id.Value);

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value[0].Street.ShouldBe("Street 1");
        result.Value[1].Street.ShouldBe("Street 2");
    }
}
