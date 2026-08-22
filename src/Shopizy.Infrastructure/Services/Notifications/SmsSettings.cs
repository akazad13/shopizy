namespace Shopizy.Infrastructure.Services.Notifications;

public class SmsSettings
{
    public const string Section = "SmsSettings";

    public bool EnableRealSms { get; set; } = false;
    public string Provider { get; set; } = "Twilio";
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromPhoneNumber { get; set; } = "+15550000000";
}
