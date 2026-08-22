using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shopizy.Infrastructure.Services.Notifications;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Services;

public class SmsServiceTests
{
    private readonly Mock<ILogger<SmsService>> _mockLogger = new();
    private readonly SmsSettings _settings = new();
    private readonly SmsService _sut;

    public SmsServiceTests()
    {
        var options = Options.Create(_settings);
        _sut = new SmsService(options, _mockLogger.Object);
    }

    [Fact]
    public async Task SendSmsAsync_ValidPhoneAndMessage_ShouldReturnTrue()
    {
        // Act
        var result = await _sut.SendSmsAsync("+1234567890", "Your order has shipped!");

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("", "Valid message")]
    [InlineData("+1234567890", "")]
    [InlineData(null, "Valid message")]
    public async Task SendSmsAsync_InvalidInput_ShouldReturnFalse(string? phone, string? message)
    {
        // Act
        var result = await _sut.SendSmsAsync(phone!, message!);

        // Assert
        result.ShouldBeFalse();
    }
}
