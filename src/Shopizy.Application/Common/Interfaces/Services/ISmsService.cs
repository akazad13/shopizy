namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// Service interface for sending SMS text notifications.
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS text message to the recipient's phone number.
    /// </summary>
    Task<bool> SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default
    );
}
