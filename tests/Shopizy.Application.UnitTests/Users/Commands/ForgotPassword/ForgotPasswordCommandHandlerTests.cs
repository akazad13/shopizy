using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.ForgotPassword;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new ForgotPasswordCommandHandler(
            _mockUserRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnEmptyString()
    {
        // Arrange
        var command = new ForgotPasswordCommand("nonexistent@test.com");

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(command.Email))
            .ReturnsAsync((Shopizy.Domain.Users.User?)null);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenUserFound_ShouldSetResetTokenAndReturnToken()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = new ForgotPasswordCommand(user.Email);

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNullOrEmpty();
        user.PasswordResetToken.ShouldBe(result.Value);
        user.PasswordResetTokenExpiry.ShouldNotBeNull();
        user.PasswordResetTokenExpiry.Value.ShouldBeGreaterThan(DateTime.UtcNow);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
