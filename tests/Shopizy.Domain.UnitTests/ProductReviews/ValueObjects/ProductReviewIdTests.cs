using Shouldly;
using Xunit;

namespace Shopizy.Domain.ProductReviews.ValueObjects.UnitTests;


public sealed class ProductReviewIdTests
{
    /// <summary>
    /// Tests that Create method successfully creates a ProductReviewId instance with a valid non-empty Guid
    /// and correctly sets the Value property.
    /// </summary>
    [Fact]
    public void Create_WithValidGuid_ShouldCreateInstanceWithCorrectValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var result = ProductReviewId.Create(guid);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(guid);
    }

    /// <summary>
    /// Tests that Create method successfully creates a ProductReviewId instance with Guid.Empty
    /// and correctly sets the Value property.
    /// </summary>
    [Fact]
    public void Create_WithEmptyGuid_ShouldCreateInstanceWithEmptyGuid()
    {
        // Arrange
        var guid = Guid.Empty;

        // Act
        var result = ProductReviewId.Create(guid);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(Guid.Empty);
    }

    /// <summary>
    /// Tests that Create method creates distinct instances when called multiple times with the same Guid value,
    /// ensuring that each instance is separate but contains the same Value.
    /// </summary>
    [Fact]
    public void Create_WithSameGuid_ShouldCreateDistinctInstancesWithSameValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var result1 = ProductReviewId.Create(guid);
        var result2 = ProductReviewId.Create(guid);

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.ShouldNotBeSameAs(result2);
        result1.Value.ShouldBe(guid);
        result2.Value.ShouldBe(guid);
        result1.ShouldBe(result2); // Value equality should be true
    }

    /// <summary>
    /// Tests that Create method works correctly with various Guid values including boundary cases.
    /// </summary>
    [Theory]
    [MemberData(nameof(GetVariousGuidValues))]
    public void Create_WithVariousGuidValues_ShouldCreateInstanceWithCorrectValue(Guid guid)
    {
        // Act
        var result = ProductReviewId.Create(guid);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(guid);
    }

    public static TheoryData<Guid> GetVariousGuidValues()
    {
        return new TheoryData<Guid>
        {
            Guid.Empty,
            Guid.NewGuid(),
            new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            new Guid("00000000-0000-0000-0000-000000000001"),
            new Guid("12345678-1234-1234-1234-123456789012")
        };
    }

    /// <summary>
    /// Tests that GetEqualityComponents returns an enumerable containing exactly one element
    /// equal to the Value property for various Guid values including Guid.Empty.
    /// </summary>
    /// <param name="guidValue">The Guid value to test with.</param>
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")] // Guid.Empty
    [InlineData("d8f3c2a1-4b6e-4f3a-9c7d-1e2f3a4b5c6d")] // Specific GUID
    [InlineData("12345678-1234-1234-1234-123456789012")] // Another specific GUID
    public void GetEqualityComponents_WithVariousGuids_ShouldReturnEnumerableContainingValue(string guidValue)
    {
        // Arrange
        var guid = Guid.Parse(guidValue);
        var productReviewId = ProductReviewId.Create(guid);

        // Act
        var components = productReviewId.GetEqualityComponents().ToList();

        // Assert
        components.ShouldNotBeNull();
        components.Count.ShouldBe(1);
        components[0].ShouldBe(guid);
    }

    /// <summary>
    /// Tests that GetEqualityComponents returns an enumerable with the correct Value
    /// when using CreateUnique factory method.
    /// </summary>
    [Fact]
    public void GetEqualityComponents_WithUniqueGuid_ShouldReturnEnumerableContainingValue()
    {
        // Arrange
        var productReviewId = ProductReviewId.CreateUnique();

        // Act
        var components = productReviewId.GetEqualityComponents().ToList();

        // Assert
        components.ShouldNotBeNull();
        components.Count.ShouldBe(1);
        components[0].ShouldBe(productReviewId.Value);
    }

    /// <summary>
    /// Tests that CreateUnique creates a valid instance with a non-empty GUID value.
    /// </summary>
    [Fact]
    public void CreateUnique_ShouldReturnValidInstanceWithNonEmptyGuid()
    {
        // Act
        var productReviewId = ProductReviewId.CreateUnique();

        // Assert
        productReviewId.ShouldNotBeNull();
        productReviewId.Value.ShouldNotBe(Guid.Empty);
    }

    /// <summary>
    /// Tests that CreateUnique generates different instances with unique GUID values
    /// when called multiple times, ensuring uniqueness of identifiers.
    /// </summary>
    [Fact]
    public void CreateUnique_CalledMultipleTimes_ShouldReturnDifferentInstances()
    {
        // Act
        var productReviewId1 = ProductReviewId.CreateUnique();
        var productReviewId2 = ProductReviewId.CreateUnique();

        // Assert
        productReviewId1.ShouldNotBe(productReviewId2);
        productReviewId1.Value.ShouldNotBe(productReviewId2.Value);
    }
}