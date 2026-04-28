using Mapster;
using Shopizy.Api.Common.Mapping;
using Shouldly;
using Xunit;

namespace Shopizy.Architecture.Tests;

public class MappingTests
{
    [Fact]
    public void Mapster_AllConfigurations_CompileSuccessfully()
    {
        // Re-create a fresh config so we don't rely on container side-effects.
        var config = new TypeAdapterConfig();
        config.Scan(typeof(DependencyInjection).Assembly);

        // Compile() throws if any registered mapping has unresolvable members,
        // ambiguous bindings, or cycles. Acts as a smoke test for mapping drift.
        var compileException = Record.Exception(() => config.Compile());
        compileException.ShouldBeNull();
    }
}
