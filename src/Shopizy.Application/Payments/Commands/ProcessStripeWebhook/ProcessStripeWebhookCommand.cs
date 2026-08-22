using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Payments.Commands.ProcessStripeWebhook;

/// <summary>
/// Command to process an incoming raw Stripe webhook payload.
/// </summary>
/// <param name="JsonPayload">The raw HTTP request body string.</param>
/// <param name="SignatureHeader">The Stripe-Signature HTTP header value.</param>
public sealed record ProcessStripeWebhookCommand(string JsonPayload, string SignatureHeader)
    : ICommand<ErrorOr<Success>>;
