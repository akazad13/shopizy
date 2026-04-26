namespace Shopizy.Contracts.Product;

public record CreateBrandRequest(string Name, string? LogoUrl, string Country);