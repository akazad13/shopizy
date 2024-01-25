using Microsoft.AspNetCore.Mvc;

namespace eStore.Api.Controllers;

[ApiController]
[Route("[Controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] summeries = new[]
    {
        "Freezing",
        "Branching",
        "Chilly",
        "Cool"
    };

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable
            .Range(1, 5)
            .Select(index =>
            {
                return new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summeries[Random.Shared.Next(summeries.Length)]
                );
            });
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
