using ErrorOr;

namespace Shopizy.Application.Users.Commands.UpdateAddress;

/// <summary>
/// Represents a command to update a user's address.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="Street">The street address.</param>
/// <param name="City">The city.</param>
/// <param name="State">The state or province.</param>
/// <param name="Country">The country.</param>
/// <param name="ZipCode">The postal/ZIP code.</param>
public record UpdateAddressCommand(
    Guid UserId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
) : MediatR.IRequest<ErrorOr<Success>>;
