namespace Shopizy.SharedKernel.Application.Models;

public class CreateSaleRequest
{
    public string CustomerId { get; set; } = null!;
    public string PaymentMethodId { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public long Amount { get; set; }
    public IReadOnlyList<string>? PaymentMethodTypes { get; set; }
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
