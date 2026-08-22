namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// Unified multi-channel notification dispatcher routing messages across Email, SMS, and Push.
/// </summary>
public interface INotificationDispatcher
{
    /// <summary>
    /// Dispatches a notification to the user across enabled channels according to preferences.
    /// </summary>
    Task DispatchNotificationAsync(
        Guid userId,
        string? email,
        string? phoneNumber,
        string subject,
        string message,
        string? targetUrl = null,
        NotificationPreferencesDto? preferences = null,
        CancellationToken cancellationToken = default
    );
}
