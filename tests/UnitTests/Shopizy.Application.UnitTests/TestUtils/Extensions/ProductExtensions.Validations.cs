using FluentAssertions;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Extensions;

public static partial class ProductExtensions
{
    public static void ValidateResult(this Product product, CreateProductCommand command)
    {
        _ = product.Name.Should().Be(command.Name);
        _ = product.Description.Should().Be(command.Description);
        _ = product.UnitPrice.Amount.Should().Be(command.UnitPrice);
        _ = product.UnitPrice.Currency.Should().Be(command.Currency);
        _ = product.Discount.Should().Be(command.Discount);
        _ = product.SKU.Should().Be(command.Sku);
        _ = product.Brand.Should().Be(command.Brand);
        _ = product.Barcode.Should().Be(command.Barcode);
        _ = product.StockQuantity.Should().Be(0);
        _ = product.Tags.Should().Be(command.Tags);
        _ = product.CategoryId.Should().BeOfType(typeof(CategoryId));
        _ = product.ModifiedOn.Should().Be(null);
    }

    public static void ValidateResult(this Product product, GetProductQuery query)
    {
        _ = product.Id.Should().BeOfType(typeof(ProductId));
        _ = product.Name.Should().BeOfType(typeof(string));
    }

    public static void ValidateResult(this Product product, UpdateProductCommand command)
    {
        _ = product.Name.Should().Be(command.Name);
        _ = product.Description.Should().Be(command.Description);
        _ = product.UnitPrice.Amount.Should().Be(command.UnitPrice);
        _ = product.UnitPrice.Currency.Should().Be(command.Currency);
        _ = product.Discount.Should().Be(command.Discount);
        _ = product.SKU.Should().Be(command.Sku);
        _ = product.Brand.Should().Be(command.Brand);
        _ = product.Barcode.Should().Be(command.Barcode);
        _ = product.Tags.Should().Be(command.Tags);
        _ = product.CategoryId.Should().BeOfType(typeof(CategoryId));
        _ = product.ModifiedOn.Should().BeAfter(product.CreatedOn);
    }
}
