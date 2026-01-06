using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Authentication
    {
        public static Error InvalidCredentials =>
            Error.Unauthorized(code: "Auth.InvalidCred", description: "Invalid credentials.");
    }
}
