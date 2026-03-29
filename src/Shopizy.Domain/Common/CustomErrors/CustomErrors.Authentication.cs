using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Authentication
    {
        public static DomainError InvalidCredentials =>
            DomainError.Unauthorized(code: "Auth.InvalidCred", description: "Invalid credentials.");
    }
}
