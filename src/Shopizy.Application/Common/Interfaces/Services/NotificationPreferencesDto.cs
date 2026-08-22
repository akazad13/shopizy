namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// DTO representing a customer's multi-channel notification preferences.
/// </summary>
public record NotificationPreferencesDto(
    bool EmailEnabled = true,
    bool SmsEnabled = true,
    bool PushEnabled = true,
    bool OrderUpdates = true,
    bool Promotions = true,
    bool PriceAlerts = true,
    bool RestockAlerts = true
);
