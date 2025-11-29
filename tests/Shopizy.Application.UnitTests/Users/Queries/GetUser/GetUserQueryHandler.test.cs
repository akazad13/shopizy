using Moq;
using Shopizy.Application.Common.Caching;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Queries.GetUser;

public class GetUserQueryHandlerTests
{
    private readonly GetUserQueryHandler _sut;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICacheHelper> _mockCacheHelper;

    public GetUserQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCacheHelper = new Mock<ICacheHelper>();
        _sut = new GetUserQueryHandler(
            _mockUserRepository.Object,
            _mockOrderRepository.Object,
            _mockCacheHelper.Object
        );
    }

    [Fact]
    public async Task Should_ReturnUserObject_WhenValidUserIdIsProvided()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user = UserFactory.UpdateAddress(user);
        var query = GetUserQueryUtils.CreateQuery();
        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{user.Id.Value}"))
            .ReturnsAsync(() => null);

        // Act
        var result = (await _sut.Handle(query, CancellationToken.None)).Match(x => x, x => null);

        // Assert
        Assert.IsType<UserDto>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Should_ReturnUserFromCache_WhenUserExistsInCache()
    {
        // Arrange
        var query = GetUserQueryUtils.CreateQuery();
        var cachedUserDto = new UserDto
        (
            Id: UserId.Create(query.UserId),
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@example.com",
            ProfileImageUrl: null,
            Phone: null,
            Address: null,
            TotalOrders: 0,
            TotalReviewed: 0,
            TotalFavorites: 0,
            TotalReturns: 0,
            CreatedOn: DateTime.UtcNow,
            ModifiedOn: null
        );

        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ReturnsAsync(cachedUserDto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(cachedUserDto, result.Value);
        _mockUserRepository.Verify(r => r.GetUserById(It.IsAny<UserId>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var query = GetUserQueryUtils.CreateQuery();
        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ReturnsAsync(() => null);

        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.User.UserNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_CacheUserData_WhenUserExistsInDatabase()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var query = GetUserQueryUtils.CreateQuery();
        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ReturnsAsync(() => null);

        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        _mockCacheHelper.Verify(
            c => c.SetAsync(
                $"user-{user.Id.Value}",
                It.IsAny<UserDto>(),
                It.IsAny<TimeSpan?>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_HandleRepositoryException_Gracefully()
    {
        // Arrange
        var query = GetUserQueryUtils.CreateQuery();
        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ReturnsAsync(() => null);

        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.User.UserNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_HandleCacheException_AndFallbackToRepository()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var query = GetUserQueryUtils.CreateQuery();

        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ThrowsAsync(new Exception("Cache service unavailable"));

        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        _mockUserRepository.Verify(r => r.GetUserById(It.IsAny<UserId>()), Times.Once);
    }
}
