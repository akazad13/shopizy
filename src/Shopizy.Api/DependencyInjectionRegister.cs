using Shopizy.Api.Common.Mapping;

namespace Shopizy.Api;

/// <summary>
/// Dependency injection register for presentation layer.
/// </summary>
public static class DependencyInjectionRegister
{
    /// <summary>
    /// Adds presentation layer services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with presentation layer services added.</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson();
        services.AddEndpointsApiExplorer().AddSwaggerGen(options =>
        {
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            var contractsXmlFile = "Shopizy.Contracts.xml";
            var contractsXmlPath = Path.Combine(AppContext.BaseDirectory, contractsXmlFile);
            options.IncludeXmlComments(contractsXmlPath);
        });
        services.AddMappings();

        return services;
    }
}
