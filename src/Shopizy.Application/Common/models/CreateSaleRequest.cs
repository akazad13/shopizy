namespace Shopizy.Application.Common.models;

public class CreateSaleRequest
{
    public string CustomerId { get; set; }
    public string PaymentMethodId { get; set; }
    public string Currency { get; set; }
    public long Amount { get; set; }
    public List<string>? PaymentMethodTypes { get; set; }
    public bool CapturePayment { get; set; } = true;
    public string? SetupFutureUsage { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }

    public void SetMetadata(Metadata metadata)
    {
        Metadata = new Dictionary<string, string> { { "orderId", metadata.OrderId } };
        if (metadata.AdditionalData != null)
        {
            foreach (var item in metadata.AdditionalData)
            {
                Metadata[item.Key] = item.Value;
            }
        }
    }
}
