using ErrorOr;
using Shopizy.Domain.Brands;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Commands.CreateBrand;

public record CreateBrandCommand(Guid UserId, string Name, string? LogoUrl, string Country)
    : ICommand<ErrorOr<Brand>>;
