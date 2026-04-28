using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace Shopizy.Architecture.Tests;

public class LayerDependencyTests
{
    private const string Domain = "Shopizy.Domain";
    private const string Application = "Shopizy.Application";
    private const string Infrastructure = "Shopizy.Infrastructure";
    private const string Api = "Shopizy.Api";
    private const string Contracts = "Shopizy.Contracts";

    [Fact]
    public void Domain_ShouldNotDependOn_OuterLayers()
    {
        var result = Types
            .InAssembly(typeof(global::Shopizy.Domain.Products.Product).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(Application, Infrastructure, Api, Contracts)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailures(result.FailingTypeNames));
    }

    [Fact]
    public void Application_ShouldNotDependOn_InfrastructureOrApi()
    {
        var result = Types
            .InAssembly(typeof(global::Shopizy.Application.DependencyInjectionRegister).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(Infrastructure, Api)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailures(result.FailingTypeNames));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        var result = Types
            .InAssembly(typeof(global::Shopizy.Infrastructure.DependencyInjectionRegister).Assembly)
            .ShouldNot()
            .HaveDependencyOn(Api)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailures(result.FailingTypeNames));
    }

    [Fact]
    public void Contracts_ShouldNotDependOn_OtherLayers()
    {
        var result = Types
            .InAssembly(typeof(global::Shopizy.Contracts.Authentication.AuthResponse).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(Application, Infrastructure, Api, Domain)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailures(result.FailingTypeNames));
    }

    private static string FormatFailures(IReadOnlyList<string>? failingTypes)
    {
        if (failingTypes is null || failingTypes.Count == 0)
        {
            return "No failing types.";
        }
        return "Failing types:\n  " + string.Join("\n  ", failingTypes);
    }
}
