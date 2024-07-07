namespace Shopizy.Contracts.User;

public class UpdateAddressRequest
{
    public string? Line { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
}
