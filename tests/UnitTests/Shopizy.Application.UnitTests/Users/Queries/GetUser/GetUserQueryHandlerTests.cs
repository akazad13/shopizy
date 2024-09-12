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
    public async Task GetUSer_WhenUserIsFound_ReturnUser()
    {
        // Arrange
        Domain.Users.User user = UserFactory.CreateUser();
        user = UserFactory.UpdateAddress(user);
        GetUserQuery query = GetUserQueryUtils.CreateQuery();
        _ = _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        // Act
        ErrorOr.ErrorOr<Domain.Users.User> result = await _handler.Handle(query, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
    }
}
