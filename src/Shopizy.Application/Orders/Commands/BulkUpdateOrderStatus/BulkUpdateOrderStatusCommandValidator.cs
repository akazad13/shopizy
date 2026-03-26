using FluentValidation;
using Shopizy.Domain.Orders.Enums;

namespace Shopizy.Application.Orders.Commands.BulkUpdateOrderStatus;

public class BulkUpdateOrderStatusCommandValidator : AbstractValidator<BulkUpdateOrderStatusCommand>
{
    public BulkUpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderIds)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one order ID must be provided.");

        RuleForEach(x => x.OrderIds)
            .NotEmpty()
            .WithMessage("Order IDs must not contain empty GUIDs.");

        RuleFor(x => x.Status)
            .Must(s => Enum.IsDefined(typeof(OrderStatus), s))
            .WithMessage("Status must be a valid OrderStatus value.");
    }
}
