using ErrorOr;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.UpdateWishlistSettings;

public record UpdateWishlistSettingsCommand(Guid UserId, string? Name, bool IsPublic) : ICommand<ErrorOr<Wishlist>>;
