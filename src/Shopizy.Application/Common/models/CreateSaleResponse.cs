namespace Shopizy.Application.Common.models;

public class CreateSaleResponse
{
    public int ResponseStatusCode { get; set; }
    public string CustomerId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentIntentId { get; set; }
    public string ObjectType { get; set; }
    public string CaptureMethod { get; set; }

    public string ChargeId { get; set; }

    public string PaymentMethodId { get; set; }

    public List<string> PaymentMethodTypes { get; set; }

    public string Status { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}
