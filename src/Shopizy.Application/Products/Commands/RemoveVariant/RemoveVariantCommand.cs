using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.RemoveVariant;

public record RemoveVariantCommand(Guid ProductId, Guid VariantId) : ICommand<ErrorOr<Deleted>>;
