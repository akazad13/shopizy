using ErrorOr;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Queries.GetWishlist;

public record GetWishlistQuery(Guid UserId) : IQuery<ErrorOr<Wishlist>>;
