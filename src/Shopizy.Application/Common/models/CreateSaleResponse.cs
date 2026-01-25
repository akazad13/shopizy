namespace Shopizy.Application.Common.models;

public class CreateSaleResponse
{
    public int ResponseStatusCode { get; set; }
    public string CustomerId { get; set; } = null!;
    public long Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string PaymentIntentId { get; set; } = null!;
    public string ObjectType { get; set; } = null!;
    public string CaptureMethod { get; set; } = null!;

    public string ChargeId { get; set; } = null!;
    public string PaymentMethodId { get; set; } = null!;
    public IReadOnlyList<string> PaymentMethodTypes { get; set; } = null!;
    public string Status { get; set; } = null!;
    public Dictionary<string, string> Metadata { get; set; } = null!;
}
