namespace Shopizy.Application.Common.models;

public class Metadata
{
    public required string OrderId { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = [];
}
