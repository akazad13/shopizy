using ErrorOr;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Commands.CreateProduct;

/// <summary>
/// Represents a command to create a new product.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="Name">The product name.</param>
/// <param name="ShortDescription">A brief description of the product.</param>
/// <param name="Description">The full product description.</param>
/// <param name="CategoryId">The category's unique identifier.</param>
/// <param name="UnitPrice">The unit price.</param>
/// <param name="Currency">The currency.</param>
/// <param name="Discount">The discount percentage.</param>
/// <param name="Sku">The stock keeping unit.</param>
/// <param name="StockQuantity">The initial stock quantity.</param>
/// <param name="Brand">The brand name.</param>
/// <param name="Colors">Available colors (comma-separated).</param>
/// <param name="Sizes">Available sizes (comma-separated).</param>
/// <param name="Tags">Product tags (comma-separated).</param>
/// <param name="Barcode">The product barcode.</param>
/// <param name="SpecificationIds">Optional list of specification IDs.</param>
public record CreateProductCommand(
    Guid UserId,
    string Name,
    string ShortDescription,
    string Description,
    Guid CategoryId,
    decimal UnitPrice,
    Currency Currency,
    decimal Discount,
    string Sku,
    int StockQuantity,
    string Brand,
    string Colors,
    string Sizes,
    string Tags,
    string Barcode,
    IList<Guid>? SpecificationIds
) : MediatR.IRequest<ErrorOr<Product>>;
