using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Products;

public class ProductVariantTests
{
    [Fact]
    public void ProductVariant_CreateAndUpdate_ShouldWork()
    {
        var price = Price.CreateNew(50, Currency.usd);
        var variant = ProductVariant.Create("Red / M", "SKU-RED-M", price, 20);

        variant.ShouldNotBeNull();
        variant.Name.ShouldBe("Red / M");
        variant.SKU.ShouldBe("SKU-RED-M");
        variant.UnitPrice.ShouldBe(price);
        variant.StockQuantity.ShouldBe(20);
        variant.IsActive.ShouldBeTrue();

        var newPrice = Price.CreateNew(60, Currency.usd);
        variant.Update("Red / L", "SKU-RED-L", newPrice, 15, false);

        variant.Name.ShouldBe("Red / L");
        variant.SKU.ShouldBe("SKU-RED-L");
        variant.UnitPrice.ShouldBe(newPrice);
        variant.StockQuantity.ShouldBe(15);
        variant.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Product_VariantsAndRatingsAndIsActive_ShouldWork()
    {
        var product = Product.Create(
            "Sample",
            "Short",
            "Long",
            CategoryId.CreateUnique(),
            "SKU123",
            10,
            Price.CreateNew(100, Currency.usd),
            null,
            null,
            "BC",
            "Red",
            "M",
            "Tag"
        );

        // Rating methods
        var rating = Rating.CreateNew(5);
        product.AddReviewRating(rating);
        product.AverageRating.NumRatings.ShouldBe(1);

        product.RemoveReviewRating(rating);
        product.AverageRating.NumRatings.ShouldBe(0);

        // SetIsActive
        product.SetIsActive(false);
        product.IsActive.ShouldBeFalse();

        // AddVariant
        var variantPrice = Price.CreateNew(120, Currency.usd);
        var variant = ProductVariant.Create("Variant 1", "VAR-1", variantPrice, 5);
        product.AddVariant(variant);
        product.ProductVariants.Count.ShouldBe(1);

        // UpdateVariant
        var updateResult = product.UpdateVariant(
            variant.Id,
            "Variant 1 Updated",
            "VAR-1-U",
            variantPrice,
            10,
            true
        );
        updateResult.IsError.ShouldBeFalse();
        updateResult.Value.Name.ShouldBe("Variant 1 Updated");

        // UpdateVariant non-existent
        var notFoundUpdate = product.UpdateVariant(
            ProductVariantId.CreateUnique(),
            "X",
            "X",
            variantPrice,
            1,
            true
        );
        notFoundUpdate.IsError.ShouldBeTrue();

        // RemoveVariant
        var removeResult = product.RemoveVariant(variant.Id);
        removeResult.IsError.ShouldBeFalse();
        product.ProductVariants.Count.ShouldBe(0);

        // RemoveVariant non-existent
        var notFoundRemove = product.RemoveVariant(ProductVariantId.CreateUnique());
        notFoundRemove.IsError.ShouldBeTrue();
    }

    [Fact]
    public void ProductVariantId_CreateUniqueAndCreate_ShouldInitialize()
    {
        var vId1 = ProductVariantId.CreateUnique();
        var raw = Guid.NewGuid();
        var vId2 = ProductVariantId.Create(raw);

        vId1.Value.ShouldNotBe(Guid.Empty);
        vId2.Value.ShouldBe(raw);
    }
}
