using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Commands.AddShipment;

namespace Shopizy.Application.UnitTests.Orders.Commands.AddShipment;

public class AddShipmentCommandValidatorTests
{
    private readonly AddShipmentCommandValidator _validator = new();

    [Fact]
    public async Task Should_HaveError_When_OrderIdIsEmpty()
    {
        var command = new AddShipmentCommand(Guid.Empty, "FedEx", "TRK123", null);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public async Task Should_HaveError_When_CarrierIsEmpty()
    {
        var command = new AddShipmentCommand(Guid.NewGuid(), "", "TRK123", null);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Carrier);
    }

    [Fact]
    public async Task Should_HaveError_When_CarrierExceedsMaxLength()
    {
        var command = new AddShipmentCommand(Guid.NewGuid(), new string('A', 101), "TRK123", null);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Carrier);
    }

    [Fact]
    public async Task Should_HaveError_When_TrackingNumberIsEmpty()
    {
        var command = new AddShipmentCommand(Guid.NewGuid(), "FedEx", "", null);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.TrackingNumber);
    }

    [Fact]
    public async Task Should_HaveError_When_TrackingNumberExceedsMaxLength()
    {
        var command = new AddShipmentCommand(Guid.NewGuid(), "FedEx", new string('T', 101), null);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.TrackingNumber);
    }

    [Fact]
    public async Task Should_NotHaveError_When_AllFieldsAreValid()
    {
        var command = new AddShipmentCommand(Guid.NewGuid(), "FedEx", "TRK123456", DateTime.UtcNow.AddDays(5));
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
