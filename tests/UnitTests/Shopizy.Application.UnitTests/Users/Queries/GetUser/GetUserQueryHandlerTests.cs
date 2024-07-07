using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Queries.GetUser;

public class GetUserQueryHandlerTests
{
    private readonly GetUserQueryHandler _handler;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public GetUserQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUserQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task GetUSer_WhenUserIsFound_ShouldReturnUser()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user = UserFactory.UpdateAddress(user);
        var query = GetUserQueryUtils.CreateQuery();
        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsError.Should().BeFalse();
    }
}
