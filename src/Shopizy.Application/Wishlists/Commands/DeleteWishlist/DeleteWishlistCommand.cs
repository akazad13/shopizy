using ErrorOr;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.DeleteWishlist;

public record DeleteWishlistCommand(UserId UserId) : ICommand<ErrorOr<Success>>;
