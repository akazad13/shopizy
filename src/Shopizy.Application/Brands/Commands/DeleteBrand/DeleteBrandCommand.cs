using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Commands.DeleteBrand;

public record DeleteBrandCommand(Guid UserId, Guid BrandId) : ICommand<ErrorOr<Success>>;