using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Infrastructure.Security.CurrentUserProvider;
using Shopizy.Infrastructure.Security.Hashing;

namespace Shopizy.Infrastructure.DependencyInjection;

public static class SecurityRegister
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordManager, PasswordManager>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => 
                policy.RequireRole("Admin"));
            
            options.AddPolicy("SellerOrAdmin", policy => 
                policy.RequireRole("Seller", "Admin"));
            
            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
