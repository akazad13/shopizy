using Shopizy.Api.Common.Mapping;
using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Api;

/// <summary>
/// Dependency injection register for presentation layer.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionRegister
{
    /// <summary>
    /// Adds presentation layer services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with presentation layer services added.</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
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

        services.AddExceptionHandler<Shopizy.Api.Common.Errors.GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
