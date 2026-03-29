using FluentValidation.TestHelper;
using Shopizy.Application.LoyaltyAccounts.Commands.EarnPoints;

namespace Shopizy.Application.UnitTests.LoyaltyAccounts.Commands.EarnPoints;

public class EarnPointsCommandValidatorTests
{
    private readonly EarnPointsCommandValidator _validator = new();

    private static EarnPointsCommand ValidCommand() =>
        new(Guid.NewGuid(), 100, "Welcome bonus");

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = ValidCommand() with { UserId = Guid.Empty };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Should_HaveError_When_PointsIsNotPositive(int points)
    {
        var command = ValidCommand() with { Points = points };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Points);
    }

    [Fact]
    public async Task Should_HaveError_When_DescriptionExceedsMaxLength()
    {
        var command = ValidCommand() with { Description = new string('A', 501) };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_AllFieldsAreValid()
    {
        var command = ValidCommand();
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_NotHaveError_When_DescriptionIsAtMaxLength()
    {
        var command = ValidCommand() with { Description = new string('A', 500) };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
