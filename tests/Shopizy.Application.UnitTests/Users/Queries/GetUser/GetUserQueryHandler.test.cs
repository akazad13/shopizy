using Moq;
using Shopizy.Application.Common.Caching;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
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
    public async Task Should_ReturnUserFromCache_When_UserExistsInCache()
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
            TotalOrders: 5,
            TotalReviewed: 2,
            TotalFavorites: 1,
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
    public async Task Should_ReturnUserFromRepository_When_UserNotInCache_And_UserExistsInRepository()
    {
        // Arrange
        var query = GetUserQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();
        
        // Ensure the query uses the same ID as the user
        // But UserFactory creates a user with a specific ID from Constants.
        // GetUserQueryUtils also uses Constants.User.Id.
        // So they should match.

        var order1 = OrderFactory.CreateOrder();
        var order2 = OrderFactory.CreateOrder();
        order2.UpdateOrderStatus(OrderStatus.Refunded);
        var order3 = OrderFactory.CreateOrder();
        order3.UpdateOrderStatus(OrderStatus.Refunded);

        var orders = new List<Order> { order1, order2, order3 };

        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ReturnsAsync(() => null);

        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        _mockOrderRepository
            .Setup(r => r.GetOrdersByUserId(user.Id))
            .Returns(orders.AsQueryable());

        _mockCacheHelper
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<UserDto>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(3, result.Value.TotalOrders);
        Assert.Equal(2, result.Value.TotalReturns); // TotalRefundedOrders maps to TotalReturns in DTO?
        
        // Let's check the mapping in Handler:
        // var totalRefundedOrders = userOrders.Count(o => o.OrderStatus == OrderStatus.Refunded);
        // var userDto = new UserDto(..., totalRefundedOrders, ...);
        // In UserDto constructor: (..., int TotalReturns, ...)
        // So yes, totalRefundedOrders is passed to TotalReturns.

        _mockCacheHelper.Verify(c => c.SetAsync($"user-{user.Id.Value}", It.IsAny<UserDto>(), It.IsAny<TimeSpan?>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnUserNotFound_When_UserNotInCache_And_UserNotExistsInRepository()
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
    public async Task Should_ReturnUserNotFound_When_RepositoryThrowsException()
    {
        // Arrange
        var query = GetUserQueryUtils.CreateQuery();

        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ReturnsAsync(() => null);

        _mockUserRepository
            .Setup(c => c.GetUserById(UserId.Create(query.UserId)))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.User.UserNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_ReturnUserNotFound_When_CacheThrowsException()
    {
        // Arrange
        var query = GetUserQueryUtils.CreateQuery();

        _mockCacheHelper
            .Setup(c => c.GetAsync<UserDto>($"user-{query.UserId}"))
            .ThrowsAsync(new Exception("Cache error"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.User.UserNotFound, result.FirstError);
    }
}
