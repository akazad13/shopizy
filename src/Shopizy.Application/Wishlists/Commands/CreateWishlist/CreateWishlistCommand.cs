using ErrorOr;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.CreateWishlist;

public record CreateWishlistCommand(Guid UserId, string? Name = null, bool IsPublic = false) : ICommand<ErrorOr<Wishlist>>;
