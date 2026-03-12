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
            // Granular Permission Policies
            options.AddPolicy("Product.Create", policy => 
                policy.RequireClaim("permissions", "create:product"));
            
            options.AddPolicy("Product.Modify", policy => 
                policy.RequireClaim("permissions", "modify:product"));
            
            options.AddPolicy("Product.Delete", policy => 
                policy.RequireClaim("permissions", "delete:product"));

            options.AddPolicy("Category.Create", policy => 
                policy.RequireClaim("permissions", "create:category"));
            
            options.AddPolicy("Category.Modify", policy => 
                policy.RequireClaim("permissions", "modify:category"));
            
            options.AddPolicy("Category.Delete", policy => 
                policy.RequireClaim("permissions", "delete:category"));
            
            // Order Policies
            options.AddPolicy("Order.Create", policy => 
                policy.RequireClaim("permissions", "create:order"));
            options.AddPolicy("Order.Get", policy => 
                policy.RequireClaim("permissions", "get:order"));
            options.AddPolicy("Order.Modify", policy => 
                policy.RequireClaim("permissions", "modify:order"));
            options.AddPolicy("Order.Delete", policy => 
                policy.RequireClaim("permissions", "delete:order"));

            // Cart Policies
            options.AddPolicy("Cart.Create", policy => 
                policy.RequireClaim("permissions", "create:cart"));
            options.AddPolicy("Cart.Get", policy => 
                policy.RequireClaim("permissions", "get:cart"));
            options.AddPolicy("Cart.Modify", policy => 
                policy.RequireClaim("permissions", "modify:cart"));
            options.AddPolicy("Cart.Delete", policy => 
                policy.RequireClaim("permissions", "delete:cart"));

            // User Policies
            options.AddPolicy("User.Get", policy => 
                policy.RequireClaim("permissions", "get:user"));
            options.AddPolicy("User.Modify", policy => 
                policy.RequireClaim("permissions", "modify:user"));

            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
