using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Infrastructure.Services;

/// <summary>
/// Email service stub that logs emails instead of sending them.
/// Replace with a real implementation (SendGrid, SMTP, etc.) when ready.
/// </summary>
public class LoggingEmailService(ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Email → To: {To} | Subject: {Subject} | Body: {Body}",
            to, subject, body
        );
        return Task.CompletedTask;
    }
}
