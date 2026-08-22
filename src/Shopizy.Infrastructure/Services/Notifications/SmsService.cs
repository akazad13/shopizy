using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Infrastructure.Services.Notifications;

public class SmsService(IOptions<SmsSettings> options, ILogger<SmsService> logger) : ISmsService
{
    private static readonly Action<ILogger, string, string, Exception?> LogSmsSent =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(1, nameof(SendSmsAsync)),
            "Dispatched SMS text to: {PhoneNumber}, Content: '{Message}'"
        );

    private readonly SmsSettings _settings = options.Value;
    private readonly ILogger<SmsService> _logger = logger;

    public Task<bool> SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(message))
        {
            return Task.FromResult(false);
        }

        var normalizedPhone = NormalizePhoneNumber(phoneNumber);
        LogSmsSent(_logger, normalizedPhone, message, null);

        return Task.FromResult(true);
    }

    private static string NormalizePhoneNumber(string phone)
    {
        var cleaned = new string(phone.Where(char.IsDigit).ToArray());
        return cleaned.StartsWith("1", StringComparison.Ordinal) ? "+" + cleaned : "+1" + cleaned;
    }
}
