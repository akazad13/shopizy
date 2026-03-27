using FluentValidation;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

public class OrderItemCommandValidator : AbstractValidator<OrderItemCommand>
{
    public OrderItemCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
