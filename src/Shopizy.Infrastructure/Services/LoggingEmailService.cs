using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Infrastructure.Services;

/// <summary>
/// Email service stub that logs emails instead of sending them.
/// Replace with a real implementation (SendGrid, SMTP, etc.) when ready.
/// </summary>
public class LoggingEmailService(ILogger<LoggingEmailService> logger) : IEmailService
{
    private static readonly Action<ILogger, string, string, string> _emailLogged = LoggerMessage.Define<string, string, string>(
        LogLevel.Information,
        new EventId(1, nameof(LoggingEmailService)),
        "Email → To: {To} | Subject: {Subject} | Body: {Body}");

    public Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        _emailLogged(logger, to, subject, body);
        return Task.CompletedTask;
    }
}
