namespace Shopizy.SharedKernel.Application.Models;

public class Metadata
{
    public required string OrderId { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = [];
}
