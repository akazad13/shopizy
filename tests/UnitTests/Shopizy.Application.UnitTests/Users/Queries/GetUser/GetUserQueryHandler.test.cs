using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Queries.GetUser;

public class GetUserQueryHandlerTests
{
    private readonly GetUserQueryHandler _sut;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public GetUserQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new GetUserQueryHandler(_mockUserRepository.Object, _mockOrderRepository.Object);
    }

    [Fact]
    public async Task ShouldReturnUserObjectWhenValidUserIdIsProvided()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user = UserFactory.UpdateAddress(user);
        var query = GetUserQueryUtils.CreateQuery();
        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        // Act
        var result = (await _sut.Handle(query, CancellationToken.None)).Match(x => x, x => null);

        // Assert
        result.Should().BeOfType<UserDto>();
        result.Should().NotBeNull();
    }

    // [Fact]
    // public async Task ShouldReturnErrorWhenInvalidUserIdIsProvided()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var invalidUserId = UserId.Create(0); // Assuming UserId is not nullable
    //     mockUserRepository.Setup(repo => repo.GetUserById(invalidUserId)).ReturnsAsync((User)null);
    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     var query = new GetUserQuery { UserId = invalidUserId.Value };

    //     // Act
    //     var result = await _sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.User.UserNotFound, result.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsWithoutDataCorruption()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var userId = UserId.Create(1);
    //     var user = new User(userId, "John Doe", "john.doe@example.com");
    //     mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     var query = new GetUserQuery { UserId = userId.Value };

    //     // Act
    //     var tasks = Enumerable
    //         .Range(0, 10)
    //         .Select(_ => _sut.Handle(query, CancellationToken.None))
    //         .ToList();

    //     await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var task in tasks)
    //     {
    //         Assert.True(task.Result.IsSuccess);
    //         Assert.Equal(user, task.Result.Value);
    //     }
    // }

    // [Fact]
    // public async Task ShouldReturnNullWhenUserIsNotFoundInDatabase()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var userId = UserId.Create(1);
    //     mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);
    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     var query = new GetUserQuery { UserId = userId.Value };

    //     // Act
    //     var result = await _sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Equal(CustomErrors.User.UserNotFound, result.Errors.Single());
    // }

    // [Fact]
    // public async Task ShouldReturnCorrectUserWhenMultipleUsersExistWithSameUserId()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var userId = UserId.Create(1);
    //     var user1 = new User(userId, "John Doe", "john.doe@example.com");
    //     var user2 = new User(userId, "Jane Doe", "jane.doe@example.com");
    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(userId))
    //         .ReturnsAsync(() =>
    //         {
    //             // Simulate multiple users with the same UserId
    //             return new List<User> { user1, user2 }.AsQueryable();
    //         });
    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     var query = new GetUserQuery { UserId = userId.Value };

    //     // Act
    //     var result = await _sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(user1, result.Value); // Assuming the first user is returned in case of multiple users
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenDatabaseConnectionIsLost()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var userId = UserId.Create(1);
    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(userId))
    //         .ThrowsAsync(new DbConnectionException());
    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     var query = new GetUserQuery { UserId = userId.Value };

    //     // Act
    //     var result = await _sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.User.UserNotFound, result.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenDatabaseQueryTimesOut()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var userId = UserId.Create(1);
    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(userId))
    //         .ThrowsAsync(new TimeoutException());
    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     var query = new GetUserQuery { UserId = userId.Value };

    //     // Act
    //     var result = await _sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.User.UserNotFound, result.Errors.Single());
    // }

    // [Fact]
    // public async Task ShouldHandleALargeNumberOfUsersWithoutPerformanceDegradation()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var users = Enumerable
    //         .Range(1, 1000)
    //         .Select(i => new User(UserId.Create(i), $"User{i}", $"user{i}@example.com"))
    //         .ToList();

    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync((UserId userId) => users.FirstOrDefault(u => u.Id == userId));

    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);

    //     // Act and Assert
    //     // Test with a random user id within the range
    //     var randomUserId = UserId.Create(new Random().Next(1, 1001));
    //     var result = await _sut.Handle(
    //         new GetUserQuery { UserId = randomUserId.Value },
    //         CancellationToken.None
    //     );

    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(users.FirstOrDefault(u => u.Id == randomUserId), result.Value);
    // }

    // [Fact]
    // public async Task ShouldReturnPaginatedListWhenPaginationParametersAreProvided()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var userId1 = UserId.Create(1);
    //     var userId2 = UserId.Create(2);
    //     var user1 = new User(userId1, "John Doe", "john.doe@example.com");
    //     var user2 = new User(userId2, "Jane Smith", "jane.smith@example.com");
    //     var users = new List<User> { user1, user2 };
    //     var pagination = new Pagination { PageNumber = 1, PageSize = 10 };
    //     mockUserRepository.Setup(repo => repo.GetUsers(It.IsAny<Pagination>())).ReturnsAsync(users);
    //     var _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     var query = new GetUserQuery { Pagination = pagination };

    //     // Act
    //     var result = await _sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(users, result.Value);
    // }

    // [Fact]
    // public void ShouldValidateInputUserId()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     GetUserQueryHandler _sut = new GetUserQueryHandler(mockUserRepository.Object);
    //     GetUserQuery queryWithNullUserId = new GetUserQuery { UserId = null };
    //     GetUserQuery queryWithEmptyUserId = new GetUserQuery { UserId = "" };

    //     // Act
    //     var resultWithNullUserId = _sut.Handle(queryWithNullUserId, CancellationToken.None);
    //     var resultWithEmptyUserId = _sut.Handle(queryWithEmptyUserId, CancellationToken.None);

    //     // Assert
    //     Assert.ThrowsAsync<ArgumentNullException>(() => resultWithNullUserId);
    //     Assert.ThrowsAsync<ArgumentException>(() => resultWithEmptyUserId);
    // }
}
