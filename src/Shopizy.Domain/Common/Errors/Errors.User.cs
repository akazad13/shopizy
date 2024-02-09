using ErrorOr;

namespace Shopizy.Domain.Common.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicatePhone =>
            Error.Conflict(code: "User.DuplicatePhone", description: "Phone is already in use.");
    }
}
