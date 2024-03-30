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
        product.Name.Should().Be(command.Name);
        product.Description.Should().Be(command.Description);
        product.UnitPrice.Amount.Should().Be(command.UnitPrice);
        product.UnitPrice.Currency.Should().Be(command.Currency);
        product.Discount.Should().Be(command.Discount);
        product.SKU.Should().Be(command.Sku);
        product.Brand.Should().Be(command.Brand);
        product.Barcode.Should().Be(command.Barcode);
        product.StockQuantity.Should().Be(0);
        product.Tags.Should().Be(command.Tags);
        product.CategoryId.Should().BeOfType(typeof(CategoryId));
        product.ModifiedOn.Should().NotBeBefore(product.CreatedOn);
    }

    public static void ValidateResult(this Product product, GetProductQuery query)
    {
        product.Id.Should().BeOfType(typeof(ProductId));
        product.Name.Should().BeOfType(typeof(string));
    }

    public static void ValidateResult(this Product product, UpdateProductCommand command)
    {
        product.Name.Should().Be(command.Name);
        product.Description.Should().Be(command.Description);
        product.UnitPrice.Amount.Should().Be(command.UnitPrice);
        product.UnitPrice.Currency.Should().Be(command.Currency);
        product.Discount.Should().Be(command.Discount);
        product.SKU.Should().Be(command.Sku);
        product.Brand.Should().Be(command.Brand);
        product.Barcode.Should().Be(command.Barcode);
        product.Tags.Should().Be(command.Tags);
        product.CategoryId.Should().BeOfType(typeof(CategoryId));
        product.ModifiedOn.Should().NotBeBefore(product.CreatedOn);
    }
}
