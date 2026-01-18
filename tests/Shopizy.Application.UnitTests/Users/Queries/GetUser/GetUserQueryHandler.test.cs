using Moq;
using Shouldly;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Caching;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Queries.GetUser;

public class GetUserQueryHandlerTestsRefactored
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICacheHelper> _mockCacheHelper;
    private readonly GetUserQueryHandler _handler;

    public GetUserQueryHandlerTestsRefactored()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCacheHelper = new Mock<ICacheHelper>();
        _handler = new GetUserQueryHandler(
            _mockUserRepository.Object,
            _mockOrderRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserInCache_ShouldReturnCachedUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserQuery(userId);
        var userDto = CreateSampleUserDto(userId);

        _mockCacheHelper.Setup(c => c.GetAsync<UserDto>(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(userDto);
    }

    private UserDto CreateSampleUserDto(Guid userId)
    {
        return new UserDto(
            UserId.Create(userId),
            "First",
            "Last",
            "test@test.com",
            null,
            "",
            null,
            0,
            0,
            0,
            0,
            DateTime.UtcNow,
            null
        );
    }
}
