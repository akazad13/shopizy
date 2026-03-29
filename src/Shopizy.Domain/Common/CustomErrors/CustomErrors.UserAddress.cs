using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class UserAddress
    {
        public static DomainError AddressNotFound =>
            DomainError.NotFound("UserAddress.AddressNotFound", "Address not found.");
    }
}
