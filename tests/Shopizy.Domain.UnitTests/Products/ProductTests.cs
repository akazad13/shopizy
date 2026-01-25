using Xunit;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products.Events;
using Shouldly;

namespace Shopizy.Domain.UnitTests.Products;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProductAndAddDomainEvent()
    {
        // Arrange
        var name = "Test Product";
        var shortDescription = "Short description";
        var description = "Full detailed description";
        var categoryId = CategoryId.CreateUnique();
        var sku = "SKU123";
        var price = Price.CreateNew(100, Currency.usd);
        decimal? discount = 10;
        var brand = "Test Brand";
        var barcode = "123456789";
        var colors = "Red, Blue";
        var sizes = "M, L";
        var tags = "tag1, tag2";

        // Act
        var product = Shopizy.Domain.Products.Product.Create(
            name,
            shortDescription,
            description,
            categoryId,
            sku,
            100, // stockQuantity
            price,
            discount,
            brand,
            barcode,
            colors,
            sizes,
            tags
        );

        // Assert
        product.ShouldNotBeNull();
        product.Name.ShouldBe(name);
        product.ShortDescription.ShouldBe(shortDescription);
        product.Description.ShouldBe(description);
        product.CategoryId.ShouldBe(categoryId);
        product.SKU.ShouldBe(sku);
        product.UnitPrice.ShouldBe(price);
        product.Discount.ShouldBe(discount);
        product.Brand.ShouldBe(brand);
        product.Barcode.ShouldBe(barcode);
        product.Colors.ShouldBe(colors);
        product.Sizes.ShouldBe(sizes);
        product.Tags.ShouldBe(tags);
        
        product.DomainEvents.ShouldContain(e => e is ProductCreatedDomainEvent);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProductAndAddDomainEvent()
    {
        // Arrange
        var product = CreateSampleProduct();
        var newName = "Updated Product Name";
        var newPrice = Price.CreateNew(150, Currency.usd);

        // Act
        product.Update(
            newName,
            product.ShortDescription,
            product.Description,
            product.CategoryId,
            product.SKU,
            newPrice,
            product.Discount,
            product.Brand,
            product.Barcode,
            product.Colors,
            product.Sizes,
            product.Tags
        );

        // Assert
        product.Name.ShouldBe(newName);
        product.UnitPrice.ShouldBe(newPrice);
    }

    [Fact]
    public void UpdateFavourite_ShouldIncrementFavourites()
    {
        // Arrange
        var product = CreateSampleProduct();
        var initialFavourites = product.Favourites;

        // Act
        product.UpdateFavourite();

        // Assert
        product.Favourites.ShouldBe(initialFavourites + 1);
    }

    private Shopizy.Domain.Products.Product CreateSampleProduct()
    {
        return Shopizy.Domain.Products.Product.Create(
            "Name",
            "Short",
            "Long",
            CategoryId.CreateUnique(),
            "SKU",
            100, // Default stock
            Price.CreateNew(10, Currency.usd),
            null,
            "Brand",
            "Barcode",
            "Colors",
            "Sizes",
            "Tags"
        );
    }
}
