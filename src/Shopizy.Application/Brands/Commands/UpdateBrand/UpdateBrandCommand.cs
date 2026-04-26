using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Commands.UpdateBrand;

public record UpdateBrandCommand(Guid UserId, Guid BrandId, string Name, string? LogoUrl, string Country)
    : ICommand<ErrorOr<Success>>;