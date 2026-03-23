using ErrorOr;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Queries.GetPublicWishlist;

public record GetPublicWishlistQuery(Guid WishlistId) : IQuery<ErrorOr<Wishlist>>;
