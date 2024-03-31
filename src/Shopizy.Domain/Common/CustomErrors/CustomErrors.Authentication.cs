using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Authentication
    {
        public static Error InvalidCredentials =>
            Error.Validation(code: "Auth.InvalidCred", description: "Invalid credentials.");
    }
}
