using Shopizy.Contracts.AuditLog;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.ProductReview;
using Shopizy.Contracts.User;
using Shouldly;
using Xunit;

namespace Shopizy.Contracts.UnitTests;

// ─────────────────────────────────────────────────────────
// AuditLogResponse
// ─────────────────────────────────────────────────────────
public class AuditLogResponseTests
{
    private static AuditLogResponse MakeSut(
        Guid? auditLogId = null,
        Guid? userId = null,
        string action = "OrderCreated",
        string entityName = "Order",
        string entityId = "order-123",
        string? oldValues = null,
        string? newValues = """{"Status":"Pending"}""",
        DateTime? timestamp = null
    ) =>
        new(
            auditLogId ?? Guid.NewGuid(),
            userId,
            action,
            entityName,
            entityId,
            oldValues,
            newValues,
            timestamp ?? DateTime.UtcNow
        );

    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ts = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        var sut = new AuditLogResponse(
            id,
            userId,
            "OrderCancelled",
            "Order",
            "ord-1",
            """{"Status":"Pending"}""",
            """{"Status":"Cancelled"}""",
            ts
        );

        sut.AuditLogId.ShouldBe(id);
        sut.UserId.ShouldBe(userId);
        sut.Action.ShouldBe("OrderCancelled");
        sut.EntityName.ShouldBe("Order");
        sut.EntityId.ShouldBe("ord-1");
        sut.OldValues.ShouldBe("""{"Status":"Pending"}""");
        sut.NewValues.ShouldBe("""{"Status":"Cancelled"}""");
        sut.Timestamp.ShouldBe(ts);
    }

    [Fact]
    public void Create_WhenUserIdIsNull_ShouldAllowNullUserId()
    {
        var sut = MakeSut(userId: null);
        sut.UserId.ShouldBeNull();
    }

    [Fact]
    public void Create_WhenOldAndNewValuesAreNull_ShouldAllowNullJsonSnapshots()
    {
        var sut = MakeSut(oldValues: null, newValues: null);
        sut.OldValues.ShouldBeNull();
        sut.NewValues.ShouldBeNull();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var a = new AuditLogResponse(id, null, "X", "Y", "Z", null, null, ts);
        var b = new AuditLogResponse(id, null, "X", "Y", "Z", null, null, ts);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentAction_ShouldNotBeEqual()
    {
        var id = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var a = MakeSut(auditLogId: id, action: "OrderCreated", timestamp: ts);
        var b = MakeSut(auditLogId: id, action: "OrderCancelled", timestamp: ts);

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = MakeSut(action: "OrderCreated");
        var updated = original with
        {
            Action = "OrderShipped",
            NewValues = """{"Status":"Shipped"}""",
        };

        original.Action.ShouldBe("OrderCreated");
        updated.Action.ShouldBe("OrderShipped");
        updated.AuditLogId.ShouldBe(original.AuditLogId);
        updated.EntityName.ShouldBe(original.EntityName);
    }

    [Fact]
    public void ToString_ShouldContainKeyProperties()
    {
        var sut = MakeSut(action: "OrderCreated", entityName: "Order");
        var str = sut.ToString();
        str.ShouldContain("OrderCreated");
        str.ShouldContain("Order");
    }
}

// ─────────────────────────────────────────────────────────
// CreateCartWithFirstProductRequest
// ─────────────────────────────────────────────────────────
public class CreateCartWithFirstProductRequestTests
{
    [Fact]
    public void Create_WithValidArguments_ShouldHoldCorrectProperties()
    {
        var productId = Guid.NewGuid();
        var sut = new CreateCartWithFirstProductRequest(productId, "Red", "M", 3);

        sut.ProductId.ShouldBe(productId);
        sut.Color.ShouldBe("Red");
        sut.Size.ShouldBe("M");
        sut.Quantity.ShouldBe(3);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var a = new CreateCartWithFirstProductRequest(id, "Blue", "L", 1);
        var b = new CreateCartWithFirstProductRequest(id, "Blue", "L", 1);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentQuantity_ShouldNotBeEqual()
    {
        var id = Guid.NewGuid();
        var a = new CreateCartWithFirstProductRequest(id, "Blue", "L", 1);
        var b = new CreateCartWithFirstProductRequest(id, "Blue", "L", 5);

        a.ShouldNotBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentProductId_ShouldNotBeEqual()
    {
        var a = new CreateCartWithFirstProductRequest(Guid.NewGuid(), "Red", "M", 1);
        var b = new CreateCartWithFirstProductRequest(Guid.NewGuid(), "Red", "M", 1);

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new CreateCartWithFirstProductRequest(Guid.NewGuid(), "Red", "M", 1);
        var updated = original with { Color = "Green", Quantity = 4 };

        original.Color.ShouldBe("Red");
        original.Quantity.ShouldBe(1);
        updated.Color.ShouldBe("Green");
        updated.Quantity.ShouldBe(4);
        updated.ProductId.ShouldBe(original.ProductId);
        updated.Size.ShouldBe(original.Size);
    }
}

// ─────────────────────────────────────────────────────────
// CartResponse  (+ CartItemResponse, CartProductResponse)
// ─────────────────────────────────────────────────────────
public class CartResponseTests
{
    private static CartProductResponse MakeProduct() =>
        new("Widget", "A great widget", 19.99m, 0m, "Acme", 50, ["img1.jpg", "img2.jpg"]);

    private static CartItemResponse MakeItem(Guid? productId = null) =>
        new(Guid.NewGuid(), productId ?? Guid.NewGuid(), "Blue", "L", 2, MakeProduct());

    [Fact]
    public void Create_WithItems_ShouldHoldCorrectProperties()
    {
        var cartId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var item = MakeItem();
        var sut = new CartResponse(cartId, userId, now, now, [item]);

        sut.CartId.ShouldBe(cartId);
        sut.UserId.ShouldBe(userId);
        sut.CartItems.ShouldNotBeNull();
        sut.CartItems.Count.ShouldBe(1);
        sut.CartItems[0].Color.ShouldBe("Blue");
        sut.CartItems[0].Size.ShouldBe("L");
        sut.CartItems[0].Quantity.ShouldBe(2);
    }

    [Fact]
    public void Create_WithEmptyItems_ShouldAllowEmptyList()
    {
        var sut = new CartResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow,
            []
        );
        sut.CartItems.ShouldNotBeNull();
        sut.CartItems.Count.ShouldBe(0);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var cartId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        // CartItems is IList<T> (reference type) — share the same instance
        // so record's structural equality check on the property succeeds.
        IList<CartItemResponse> sharedItems = [];
        var a = new CartResponse(cartId, userId, ts, ts, sharedItems);
        var b = new CartResponse(cartId, userId, ts, ts, sharedItems);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentCartId_ShouldNotBeEqual()
    {
        var ts = DateTime.UtcNow;
        var a = new CartResponse(Guid.NewGuid(), Guid.NewGuid(), ts, ts, []);
        var b = new CartResponse(Guid.NewGuid(), Guid.NewGuid(), ts, ts, []);

        a.ShouldNotBe(b);
    }

    [Fact]
    public void CartItemResponse_ShouldHoldProductDetails()
    {
        var productId = Guid.NewGuid();
        var product = MakeProduct();
        var item = new CartItemResponse(Guid.NewGuid(), productId, "Red", "S", 1, product);

        item.ProductId.ShouldBe(productId);
        item.Color.ShouldBe("Red");
        item.Size.ShouldBe("S");
        item.Quantity.ShouldBe(1);
        item.Product.Name.ShouldBe("Widget");
        item.Product.Price.ShouldBe(19.99m);
        item.Product.StockQuantity.ShouldBe(50);
    }

    [Fact]
    public void CartProductResponse_WhenImagesNull_ShouldAllowNull()
    {
        var product = new CartProductResponse("Gizmo", "Desc", 9.99m, 1m, "Brand", 5, null);
        product.ProductImages.ShouldBeNull();
    }

    [Fact]
    public void CartProductResponse_WithImages_ShouldHoldImageList()
    {
        var product = MakeProduct();
        product.ProductImages.ShouldNotBeNull();
        product.ProductImages!.Count.ShouldBe(2);
        product.ProductImages.ShouldContain("img1.jpg");
    }
}

// ─────────────────────────────────────────────────────────
// ProductReviewResponse
// ─────────────────────────────────────────────────────────
public class ProductReviewResponseTests
{
    [Fact]
    public void Create_WithValidArguments_ShouldHoldCorrectProperties()
    {
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var created = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        var sut = new ProductReviewResponse(
            reviewId,
            userId,
            "Alice",
            4.5m,
            "Great product!",
            created
        );

        sut.ReviewId.ShouldBe(reviewId);
        sut.UserId.ShouldBe(userId);
        sut.UserName.ShouldBe("Alice");
        sut.Rating.ShouldBe(4.5m);
        sut.Comment.ShouldBe("Great product!");
        sut.CreatedOn.ShouldBe(created);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var rid = Guid.NewGuid();
        var uid = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var a = new ProductReviewResponse(rid, uid, "Bob", 5m, "Love it", ts);
        var b = new ProductReviewResponse(rid, uid, "Bob", 5m, "Love it", ts);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentRating_ShouldNotBeEqual()
    {
        var rid = Guid.NewGuid();
        var uid = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var a = new ProductReviewResponse(rid, uid, "Bob", 5m, "Love it", ts);
        var b = new ProductReviewResponse(rid, uid, "Bob", 3m, "Love it", ts);

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new ProductReviewResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Carol",
            3m,
            "OK",
            DateTime.UtcNow
        );
        var updated = original with { Rating = 5m, Comment = "Changed my mind!" };

        original.Rating.ShouldBe(3m);
        updated.Rating.ShouldBe(5m);
        updated.Comment.ShouldBe("Changed my mind!");
        updated.ReviewId.ShouldBe(original.ReviewId);
        updated.UserName.ShouldBe(original.UserName);
    }

    [Fact]
    public void Rating_BoundaryValues_ShouldBeStored()
    {
        var r1 = new ProductReviewResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "U",
            1m,
            "Min",
            DateTime.UtcNow
        );
        var r5 = new ProductReviewResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "U",
            5m,
            "Max",
            DateTime.UtcNow
        );

        r1.Rating.ShouldBe(1m);
        r5.Rating.ShouldBe(5m);
    }

    [Fact]
    public void ToString_ShouldContainKeyProperties()
    {
        var sut = new ProductReviewResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Dave",
            4m,
            "Nice",
            DateTime.UtcNow
        );
        var str = sut.ToString();
        str.ShouldContain("Dave");
        str.ShouldContain("4");
    }
}

// ─────────────────────────────────────────────────────────
// AddAddressRequest
// ─────────────────────────────────────────────────────────
public class AddAddressRequestTests
{
    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var sut = new AddAddressRequest("123 Main St", "Springfield", "IL", "USA", "62701", true);

        sut.Street.ShouldBe("123 Main St");
        sut.City.ShouldBe("Springfield");
        sut.State.ShouldBe("IL");
        sut.Country.ShouldBe("USA");
        sut.ZipCode.ShouldBe("62701");
        sut.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void IsDefault_WhenFalse_ShouldStoreCorrectly()
    {
        var sut = new AddAddressRequest("1 Other St", "City", "ST", "Country", "00000", false);
        sut.IsDefault.ShouldBeFalse();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = new AddAddressRequest("A", "B", "C", "D", "E", true);
        var b = new AddAddressRequest("A", "B", "C", "D", "E", true);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentZipCode_ShouldNotBeEqual()
    {
        var a = new AddAddressRequest("A", "B", "C", "D", "11111", true);
        var b = new AddAddressRequest("A", "B", "C", "D", "99999", true);

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new AddAddressRequest("Old St", "OldCity", "OS", "OC", "00000", false);
        var updated = original with { Street = "New St", IsDefault = true };

        original.Street.ShouldBe("Old St");
        original.IsDefault.ShouldBeFalse();
        updated.Street.ShouldBe("New St");
        updated.IsDefault.ShouldBeTrue();
        updated.City.ShouldBe(original.City);
    }
}

// ─────────────────────────────────────────────────────────
// AddUserAddressRequest
// ─────────────────────────────────────────────────────────
public class AddUserAddressRequestTests
{
    [Fact]
    public void Create_WithExplicitDefault_ShouldHoldCorrectValues()
    {
        var sut = new AddUserAddressRequest(
            "456 Elm St",
            "Shelbyville",
            "TN",
            "USA",
            "37160",
            true
        );

        sut.Street.ShouldBe("456 Elm St");
        sut.City.ShouldBe("Shelbyville");
        sut.State.ShouldBe("TN");
        sut.Country.ShouldBe("USA");
        sut.ZipCode.ShouldBe("37160");
        sut.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void Create_WithoutIsDefault_ShouldDefaultToFalse()
    {
        var sut = new AddUserAddressRequest("789 Oak Ave", "Capital", "CA", "USA", "90210");
        sut.IsDefault.ShouldBeFalse();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = new AddUserAddressRequest("St", "City", "ST", "US", "12345", false);
        var b = new AddUserAddressRequest("St", "City", "ST", "US", "12345", false);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentIsDefault_ShouldNotBeEqual()
    {
        var a = new AddUserAddressRequest("St", "City", "ST", "US", "12345", false);
        var b = new AddUserAddressRequest("St", "City", "ST", "US", "12345", true);

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new AddUserAddressRequest("Old", "OC", "OS", "OX", "00000");
        var updated = original with { ZipCode = "99999", IsDefault = true };

        original.ZipCode.ShouldBe("00000");
        updated.ZipCode.ShouldBe("99999");
        updated.IsDefault.ShouldBeTrue();
        updated.Street.ShouldBe(original.Street);
    }
}

// ─────────────────────────────────────────────────────────
// UpdateUserAddressRequest
// ─────────────────────────────────────────────────────────
public class UpdateUserAddressRequestTests
{
    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var sut = new UpdateUserAddressRequest("10 Down St", "London", "England", "UK", "SW1A 2AA");

        sut.Street.ShouldBe("10 Down St");
        sut.City.ShouldBe("London");
        sut.State.ShouldBe("England");
        sut.Country.ShouldBe("UK");
        sut.ZipCode.ShouldBe("SW1A 2AA");
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = new UpdateUserAddressRequest("S", "C", "ST", "CO", "ZIP");
        var b = new UpdateUserAddressRequest("S", "C", "ST", "CO", "ZIP");

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentStreet_ShouldNotBeEqual()
    {
        var a = new UpdateUserAddressRequest("Old St", "C", "ST", "CO", "ZIP");
        var b = new UpdateUserAddressRequest("New St", "C", "ST", "CO", "ZIP");

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new UpdateUserAddressRequest("Old St", "OldCity", "OS", "OC", "00000");
        var updated = original with { City = "NewCity", ZipCode = "11111" };

        original.City.ShouldBe("OldCity");
        updated.City.ShouldBe("NewCity");
        updated.ZipCode.ShouldBe("11111");
        updated.Street.ShouldBe(original.Street);
        updated.Country.ShouldBe(original.Country);
    }
}

// ─────────────────────────────────────────────────────────
// UserAddressResponse
// ─────────────────────────────────────────────────────────
public class UserAddressResponseTests
{
    private static UserAddressResponse MakeSut(Guid? addressId = null, bool isDefault = true) =>
        new(
            addressId ?? Guid.NewGuid(),
            "1 Test St",
            "Testville",
            "TS",
            "Testland",
            "T1 1TT",
            isDefault,
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        );

    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var id = Guid.NewGuid();
        var created = new DateTime(2026, 3, 15, 8, 30, 0, DateTimeKind.Utc);

        var sut = new UserAddressResponse(
            id,
            "5 Park Ave",
            "NYC",
            "NY",
            "USA",
            "10001",
            true,
            created
        );

        sut.AddressId.ShouldBe(id);
        sut.Street.ShouldBe("5 Park Ave");
        sut.City.ShouldBe("NYC");
        sut.State.ShouldBe("NY");
        sut.Country.ShouldBe("USA");
        sut.ZipCode.ShouldBe("10001");
        sut.IsDefault.ShouldBeTrue();
        sut.CreatedOn.ShouldBe(created);
    }

    [Fact]
    public void IsDefault_WhenFalse_ShouldStoreCorrectly()
    {
        var sut = MakeSut(isDefault: false);
        sut.IsDefault.ShouldBeFalse();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var a = MakeSut(addressId: id);
        var b = MakeSut(addressId: id);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentAddressId_ShouldNotBeEqual()
    {
        var a = MakeSut(addressId: Guid.NewGuid());
        var b = MakeSut(addressId: Guid.NewGuid());

        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = MakeSut();
        var updated = original with { City = "New City", IsDefault = false };

        original.City.ShouldBe("Testville");
        original.IsDefault.ShouldBeTrue();
        updated.City.ShouldBe("New City");
        updated.IsDefault.ShouldBeFalse();
        updated.AddressId.ShouldBe(original.AddressId);
        updated.Street.ShouldBe(original.Street);
    }

    [Fact]
    public void ToString_ShouldContainKeyProperties()
    {
        var sut = MakeSut();
        var str = sut.ToString();
        str.ShouldContain("1 Test St");
        str.ShouldContain("Testville");
    }
}
