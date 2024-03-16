using FluentAssertions;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Domain.Products;

namespace Shopizy.Application.UnitTests.TestUtils.Products.Extensions;

public static partial class ProductExtensions
{
    public static void ValidateCreatedForm(this Product product, CreateProductCommand command)
    {
        product.Name.Should().Be(command.Name);
        product.Description.Should().Be(command.Description);
    }
}
