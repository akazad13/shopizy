using FluentValidation;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DeliveryChargeAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OrderItems).NotEmpty();
        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCommandValidator());
        RuleFor(x => x.ShippingAddress).NotNull().SetValidator(new AddressCommandValidator());
    }
}
