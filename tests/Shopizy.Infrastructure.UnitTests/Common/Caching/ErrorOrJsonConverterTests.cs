using System.Text.Json;
using ErrorOr;
using Shopizy.Infrastructure.Common.Caching;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Common.Caching;

public class ErrorOrJsonConverterTests
{
    private class TestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void ErrorOrJsonConverter_ShouldSerializeAndDeserializeSuccessValue()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ErrorOrConverterFactory());

        ErrorOr<TestDto> original = new TestDto { Id = 42, Name = "Test" };

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<ErrorOr<TestDto>>(json, options);

        // Assert
        json.ShouldContain("\"Id\":42");
        json.ShouldContain("\"Name\":\"Test\"");

        deserialized.IsError.ShouldBeFalse();
        deserialized.Value.Id.ShouldBe(42);
        deserialized.Value.Name.ShouldBe("Test");
    }

    [Fact]
    public void ErrorOrJsonConverter_WhenError_ShouldSerializeDefaultValue()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ErrorOrConverterFactory());

        ErrorOr<TestDto> original = Error.Failure("Test.Error", "Something went wrong");

        // Act
        var json = JsonSerializer.Serialize(original, options);

        // Assert
        json.ShouldBe("null");
    }
}
