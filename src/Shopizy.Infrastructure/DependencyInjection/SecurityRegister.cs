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
                policy.RequireClaim("permissions", "create:product").RequireRole("Admin"));
            
            options.AddPolicy("Product.Modify", policy => 
                policy.RequireClaim("permissions", "modify:product").RequireRole("Admin"));
            
            options.AddPolicy("Product.Delete", policy => 
                policy.RequireClaim("permissions", "delete:product").RequireRole("Admin"));

            options.AddPolicy("Category.Create", policy => 
                policy.RequireClaim("permissions", "create:category").RequireRole("Admin"));
            
            options.AddPolicy("Category.Modify", policy => 
                policy.RequireClaim("permissions", "modify:category").RequireRole("Admin"));
            
            options.AddPolicy("Category.Delete", policy => 
                policy.RequireClaim("permissions", "delete:category").RequireRole("Admin"));
            
            // Order Policies
            options.AddPolicy("Order.Create", policy => 
                policy.RequireClaim("permissions", "create:order")); // Customer can create order
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

            // Admin Policies
            options.AddPolicy("Admin", policy => 
                policy.RequireRole("Admin"));

            options.AddPolicy("Admin.View", policy => 
                policy.RequireRole("Admin"));

            options.AddPolicy("Admin.ViewOrder", policy => 
                policy.RequireRole("Admin"));

            options.AddPolicy("Admin.ViewOrders", policy => 
                policy.RequireRole("Admin"));

            options.AddPolicy("Admin.ViewUsers", policy => 
                policy.RequireRole("Admin"));

            options.AddPolicy("Admin.UpdateOrderStatus", policy => 
                policy.RequireRole("Admin"));

            options.AddPolicy("Admin.UpdateUserRole", policy => 
                policy.RequireRole("Admin"));

            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
