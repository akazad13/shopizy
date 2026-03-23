using FluentValidation;

namespace Shopizy.Application.Orders.Commands.BulkUpdateOrderStatus;

public class BulkUpdateOrderStatusCommandValidator : AbstractValidator<BulkUpdateOrderStatusCommand>
{
    public BulkUpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderIds).NotEmpty();
    }
}
