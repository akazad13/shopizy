namespace Shopizy.SharedKernel.Application.Models;

/// <summary>
/// Represents the type of asynchronous payment webhook event.
/// </summary>
public enum PaymentWebhookEventType
{
    /// <summary>
    /// Unrecognized or unhandled event.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Payment was successfully authorized/captured.
    /// </summary>
    PaymentSucceeded = 1,

    /// <summary>
    /// Payment attempt failed.
    /// </summary>
    PaymentFailed = 2,

    /// <summary>
    /// Charge was refunded.
    /// </summary>
    ChargeRefunded = 3,
}

/// <summary>
/// Normalized payment webhook event data parsed from external payment providers.
/// </summary>
/// <param name="EventType">The normalized event type.</param>
/// <param name="OrderId">The associated system Order ID if present in metadata.</param>
/// <param name="ChargeId">The provider charge identifier.</param>
/// <param name="CustomerId">The provider customer identifier.</param>
/// <param name="PaymentIntentId">The provider payment intent identifier.</param>
/// <param name="FailureMessage">Diagnostic error message if payment failed.</param>
public sealed record PaymentWebhookEvent(
    PaymentWebhookEventType EventType,
    string? OrderId,
    string? ChargeId,
    string? CustomerId,
    string? PaymentIntentId,
    string? FailureMessage
);
