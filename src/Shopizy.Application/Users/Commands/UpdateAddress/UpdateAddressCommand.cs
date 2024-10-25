using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Users.Commands.UpdateAddress;

[Authorize(Permissions = Permissions.User.Modify, Policies = Policy.SelfOrAdmin)]
public record UpdateAddressCommand(
    Guid UserId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
) : IAuthorizeableRequest<IResult<GenericResponse>>;
