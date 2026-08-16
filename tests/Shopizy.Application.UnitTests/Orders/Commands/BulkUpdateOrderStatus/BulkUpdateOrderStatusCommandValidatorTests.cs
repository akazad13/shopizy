using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Commands.BulkUpdateOrderStatus;
using Shopizy.Domain.Orders.Enums;

namespace Shopizy.Application.UnitTests.Orders.Commands.BulkUpdateOrderStatus;

public class BulkUpdateOrderStatusCommandValidatorTests
{
    private readonly BulkUpdateOrderStatusCommandValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenCommandIsValid()
    {
        // Arrange
        var command = new BulkUpdateOrderStatusCommand(
            new List<Guid> { Guid.NewGuid() },
            (int)OrderStatus.Delivered
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenOrderIdsIsEmpty()
    {
        // Arrange
        var command = new BulkUpdateOrderStatusCommand(
            new List<Guid>(),
            (int)OrderStatus.Delivered
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderIds);
    }

    [Fact]
    public void Should_HaveError_WhenOrderIdsContainsEmptyGuid()
    {
        // Arrange
        var command = new BulkUpdateOrderStatusCommand(
            new List<Guid> { Guid.Empty },
            (int)OrderStatus.Delivered
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderIds);
    }

    [Fact]
    public void Should_HaveError_WhenStatusIsInvalidEnum()
    {
        // Arrange
        var command = new BulkUpdateOrderStatusCommand(new List<Guid> { Guid.NewGuid() }, 9999);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
