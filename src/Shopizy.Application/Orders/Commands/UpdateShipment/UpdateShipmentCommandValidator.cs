using FluentValidation;
using Shopizy.Domain.Orders.Enums;

namespace Shopizy.Application.Orders.Commands.UpdateShipment;

public class UpdateShipmentCommandValidator : AbstractValidator<UpdateShipmentCommand>
{
    public UpdateShipmentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Carrier).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TrackingNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Status)
            .Must(s => Enum.IsDefined(typeof(ShipmentStatus), s))
            .WithMessage("Status must be a valid ShipmentStatus value.");
    }
}
