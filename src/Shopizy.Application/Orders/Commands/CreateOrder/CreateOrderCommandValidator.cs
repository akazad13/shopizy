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

public class OrderItemCommandValidator : AbstractValidator<OrderItemCommand>
{
    public OrderItemCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class AddressCommandValidator : AbstractValidator<AddressCommand>
{
    public AddressCommandValidator()
    {
        RuleFor(x => x.Street).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(50);
        RuleFor(x => x.State).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(10);
    }
}
