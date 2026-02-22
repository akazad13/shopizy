using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Infrastructure.Carts.Persistence;
using Shopizy.Infrastructure.Categories.Persistence;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Common.Persistence.Interceptors;
using Shopizy.Infrastructure.Orders.Persistence;
using Shopizy.Infrastructure.Payments.Persistence;
using Shopizy.Infrastructure.Permissions.Persistence;
using Shopizy.Infrastructure.ProductReviews.Persistence;
using Shopizy.Infrastructure.Products.Persistence;
using Shopizy.Infrastructure.PromoCodes.Persistence;
using Shopizy.Infrastructure.Users.Persistence;
using Shopizy.Infrastructure.Services;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;

namespace Shopizy.Infrastructure.DependencyInjection;

public static class PersistenceRegister
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IAppDbContext, AppDbContext>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>())
            .AddScoped<DbMigrationsHelper>()
            .AddScoped<UpdateAuditableEntitiesInterceptor>();

        if (configuration.GetValue<bool>("UsePostgreSql"))
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .AddInterceptors(interceptor);
            });
        }
        else
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                    .AddInterceptors(interceptor);
            });
        }

        return services.AddRepositories();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<ICartRepository, CartRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IPaymentRepository, PaymentRepository>()
            .AddScoped<IProductReviewRepository, ProductReviewRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IPromoCodeRepository, PromoCodeRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IPermissionRepository, PermissionRepository>();

        return services;
    }
}
