using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Commands.UpdateShipment;

namespace Shopizy.Application.UnitTests.Orders.Commands.UpdateShipment;

public class UpdateShipmentCommandValidatorTests
{
    private readonly UpdateShipmentCommandValidator _validator = new();

    [Fact]
    public async Task Should_HaveError_When_OrderIdIsEmpty()
    {
        var command = new UpdateShipmentCommand(Guid.Empty, "FedEx", "TRK123", null, 1);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public async Task Should_HaveError_When_CarrierIsEmpty()
    {
        var command = new UpdateShipmentCommand(Guid.NewGuid(), "", "TRK123", null, 1);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Carrier);
    }

    [Fact]
    public async Task Should_HaveError_When_CarrierExceedsMaxLength()
    {
        var command = new UpdateShipmentCommand(Guid.NewGuid(), new string('A', 101), "TRK123", null, 1);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Carrier);
    }

    [Fact]
    public async Task Should_HaveError_When_TrackingNumberIsEmpty()
    {
        var command = new UpdateShipmentCommand(Guid.NewGuid(), "FedEx", "", null, 1);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.TrackingNumber);
    }

    [Fact]
    public async Task Should_HaveError_When_StatusIsInvalid()
    {
        var command = new UpdateShipmentCommand(Guid.NewGuid(), "FedEx", "TRK123", null, 999);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public async Task Should_NotHaveError_When_AllFieldsAreValid()
    {
        var command = new UpdateShipmentCommand(Guid.NewGuid(), "FedEx", "TRK123456", null, 1);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
