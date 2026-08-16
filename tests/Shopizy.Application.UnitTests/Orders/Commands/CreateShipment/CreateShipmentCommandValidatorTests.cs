using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Commands.CreateShipment;

namespace Shopizy.Application.UnitTests.Orders.Commands.CreateShipment;

public class CreateShipmentCommandValidatorTests
{
    private readonly CreateShipmentCommandValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateShipmentCommand(
            Guid.NewGuid(),
            "FedEx",
            "TRACK123",
            DateTime.UtcNow.AddDays(2)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenOrderIdIsEmpty()
    {
        // Arrange
        var command = new CreateShipmentCommand(
            Guid.Empty,
            "FedEx",
            "TRACK123",
            DateTime.UtcNow.AddDays(2)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public void Should_HaveError_WhenCarrierIsEmpty()
    {
        // Arrange
        var command = new CreateShipmentCommand(
            Guid.NewGuid(),
            "",
            "TRACK123",
            DateTime.UtcNow.AddDays(2)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Carrier);
    }

    [Fact]
    public void Should_HaveError_WhenTrackingNumberIsEmpty()
    {
        // Arrange
        var command = new CreateShipmentCommand(
            Guid.NewGuid(),
            "FedEx",
            "",
            DateTime.UtcNow.AddDays(2)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TrackingNumber);
    }
}
