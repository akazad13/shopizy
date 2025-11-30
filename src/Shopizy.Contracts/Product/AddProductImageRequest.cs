using Microsoft.AspNetCore.Http;

namespace Shopizy.Contracts.Product;

/// <summary>
/// Represents a request to add an image to a product.
/// </summary>
/// <param name="File">The image file to upload.</param>
public record AddProductImageRequest(IFormFile? File);
