using ErrorOr;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.RemoveWishlistItem;

public record RemoveWishlistItemCommand(UserId UserId, ProductId ProductId)
    : ICommand<ErrorOr<Wishlist>>;
