using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Queries.GetUsers;
using Shopizy.Domain.Users;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Queries.GetUsers;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUsersExist_ShouldReturnPagedResult()
    {
        // Arrange
        var user1 = UserFactory.CreateUser();
        var user2 = UserFactory.CreateUser();
        var usersList = new List<User> { user1, user2 };
        var query = new GetUsersQuery(1, 10);

        _mockUserRepository
            .Setup(r => r.ListUsersAsync(query.PageNumber, query.PageSize))
            .ReturnsAsync(usersList);
        _mockUserRepository.Setup(r => r.GetTotalUsersCountAsync()).ReturnsAsync(2);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.PageNumber.ShouldBe(1);
        result.Value.PageSize.ShouldBe(10);
        result.Value.TotalCount.ShouldBe(2);
    }
}
