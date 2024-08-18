namespace Shopizy.Application.Common.models;

public record CustomerResource(string CustomerId, string Email, string Name);

public record ChargeResource(
    string ChargeId,
    string Currency,
    long Amount,
    string CustomerId,
    string ReceiptEmail,
    string Description
);

public record CheckoutSession(string SessionId);
