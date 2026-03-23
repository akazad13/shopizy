using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class UserAddress
    {
        public static Error AddressNotFound =>
            Error.NotFound("UserAddress.AddressNotFound", "Address not found.");
    }
}
