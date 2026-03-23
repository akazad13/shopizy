using FluentValidation;

namespace Shopizy.Application.Orders.Commands.AddShipment;

public class AddShipmentCommandValidator : AbstractValidator<AddShipmentCommand>
{
    public AddShipmentCommandValidator()
    {
        RuleFor(x => x.Carrier).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TrackingNumber).NotEmpty().MaximumLength(100);
    }
}
