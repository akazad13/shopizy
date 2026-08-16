using System;
using System.IO;
using System.Text.RegularExpressions;

var dir = @"d:\Projects\akazad13\shopizy\tests\Shopizy.Api.IntegrationTests";
var files = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);

foreach (var file in files)
{
    var content = File.ReadAllText(file);
    bool changed = false;

    // Pattern 1: Named parameters
    if (Regex.IsMatch(content, @"(PromoCode:\s*"".*?"",)"))
    {
        content = Regex.Replace(
            content,
            @"(PromoCode:\s*"".*?"",)",
            "$1\n            GiftCardCode: null,"
        );
        changed = true;
    }

    // Pattern 2: Positional parameters (like in AdminOrderTests.cs and CrossTenantAuthorizationTests.cs)
    // new CreateOrderRequest(\n                "",\n                1,
    // Note: CrossTenantAuthorizationTests uses "SUMMER24" etc.
    if (Regex.IsMatch(content, @"new CreateOrderRequest\(\s*""[^""]*"",\s*\d+,"))
    {
        content = Regex.Replace(
            content,
            @"(new CreateOrderRequest\(\s*""[^""]*"",)(\s*\d+,)",
            "$1\n                null,$2"
        );
        changed = true;
    }

    if (changed)
    {
        File.WriteAllText(file, content);
    }
}
