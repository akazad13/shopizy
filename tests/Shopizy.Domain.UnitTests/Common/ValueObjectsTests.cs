using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Common;

public class ValueObjectsTests
{
    [Fact]
    public void AverageRating_CreateNewAndMutate_ShouldCalculateCorrectly()
    {
        // Arrange
        var avg = AverageRating.CreateNew();
        avg.Value.ShouldBe(0);
        avg.NumRatings.ShouldBe(0);

        // Add 5-star rating
        avg.AddNewRating(Rating.CreateNew(5));
        avg.Value.ShouldBe(5);
        avg.NumRatings.ShouldBe(1);

        // Add 3-star rating
        avg.AddNewRating(Rating.CreateNew(3));
        avg.Value.ShouldBe(4);
        avg.NumRatings.ShouldBe(2);

        // Remove 5-star rating (leaving 1 rating of 3-star)
        avg.RemoveRating(Rating.CreateNew(5));
        avg.Value.ShouldBe(3);
        avg.NumRatings.ShouldBe(1);

        // Remove final rating (when NumRatings <= 1)
        avg.RemoveRating(Rating.CreateNew(3));
        avg.Value.ShouldBe(0);
        avg.NumRatings.ShouldBe(0);

        var avg1 = AverageRating.CreateNew(4.5m, 10);
        var avg2 = AverageRating.CreateNew(4.5m, 10);
        avg1.ShouldBe(avg2);
        avg1.GetHashCode().ShouldBe(avg2.GetHashCode());
    }

    [Fact]
    public void Price_CreateNew_ShouldSetPropertiesAndCheckEquality()
    {
        // Arrange
        var price1 = Price.CreateNew(100.50m, Currency.usd);
        var price2 = Price.CreateNew(100.50m, Currency.usd);
        var price3 = Price.CreateNew(200.00m, Currency.usd);

        // Assert
        price1.Amount.ShouldBe(100.50m);
        price1.Currency.ShouldBe(Currency.usd);

        price1.ShouldBe(price2);
        price1.ShouldNotBe(price3);
    }

    [Fact]
    public void Rating_CreateNew_ShouldSetPropertiesAndCheckEquality()
    {
        // Arrange
        var rating1 = Rating.CreateNew(4.5m);
        var rating2 = Rating.CreateNew(4.5m);
        var rating3 = Rating.CreateNew(5.0m);

        // Assert
        rating1.Value.ShouldBe(4.5m);
        rating1.ShouldBe(rating2);
        rating1.ShouldNotBe(rating3);
    }
}
