using ErrorOr;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.CreateProduct;

/// <summary>
/// Creates a new product. Carries domain value objects (<see cref="Price"/>, <see cref="CategoryId"/>,
/// <see cref="BrandId"/>) instead of primitives — the mapping layer converts the contract DTO
/// into typed inputs so handlers don't repeat construction logic.
/// </summary>
public record CreateProductCommand(
    Guid UserId,
    string Name,
    string ShortDescription,
    string Description,
    CategoryId CategoryId,
    Price UnitPrice,
    decimal Discount,
    string Sku,
    int StockQuantity,
    BrandId? BrandId,
    string Colors,
    string Sizes,
    string Tags,
    string Barcode,
    IList<Guid>? SpecificationIds
) : ICommand<ErrorOr<Product>>;
