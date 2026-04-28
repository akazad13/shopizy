using System.Text.RegularExpressions;

namespace Shopizy.SharedKernel.Application.Logging;

/// <summary>
/// Redacts common PII / secrets from free-form strings before they reach log sinks.
/// Used at boundaries that emit attacker-controlled or user-supplied text (e.g., exception messages,
/// validation feedback) where a destructuring policy would otherwise be needed.
/// </summary>
public static partial class LogSanitizer
{
    private const string s_emailMask = "[email]";
    private const string s_phoneMask = "[phone]";
    private const string s_tokenMask = "[token]";
    private const string s_cardMask = "[card]";

    [GeneratedRegex(
        @"[\w.+-]+@[\w-]+\.[\w.-]+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
    )]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"\+?\d[\d\s().-]{8,}\d", RegexOptions.CultureInvariant)]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"\b(?:\d[ -]?){13,19}\b", RegexOptions.CultureInvariant)]
    private static partial Regex CardRegex();

    [GeneratedRegex(
        @"(?i)\b(bearer|token|apikey|api_key|authorization)[=:\s]+([A-Za-z0-9._\-]+)",
        RegexOptions.CultureInvariant
    )]
    private static partial Regex TokenRegex();

    /// <summary>
    /// Returns a copy of <paramref name="value"/> with email addresses, phone numbers,
    /// payment-card-shaped digit runs, and bearer/token segments masked.
    /// </summary>
    public static string Sanitize(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var redacted = CardRegex().Replace(value, s_cardMask);
        redacted = EmailRegex().Replace(redacted, s_emailMask);
        redacted = TokenRegex().Replace(redacted, m => $"{m.Groups[1].Value}={s_tokenMask}");
        redacted = PhoneRegex().Replace(redacted, s_phoneMask);
        return redacted;
    }
}
