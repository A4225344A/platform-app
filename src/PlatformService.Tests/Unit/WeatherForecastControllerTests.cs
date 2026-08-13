using Microsoft.AspNetCore.Mvc;
using PlatformService.Controllers;
using PlatformService.Models;
using PlatformService.Services;

namespace PlatformService.Tests.Unit;

public class WeatherForecastControllerTests
{
    private sealed class FakeWeatherForecastService : IWeatherForecastService
    {
        public WeatherForecast[] Result { get; init; } = Array.Empty<WeatherForecast>();

        public WeatherForecast[] GetForecast() => Result;
    }

    [Fact]
    public void Get_ReturnsOkWithForecastFromService()
    {
        var fakeService = new FakeWeatherForecastService
        {
            Result = new[] { new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), 10, "Cool") },
        };
        var controller = new WeatherForecastController(fakeService);

        var result = controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var forecast = Assert.IsType<WeatherForecast[]>(okResult.Value);
        Assert.Single(forecast);
        Assert.Equal("Cool", forecast[0].Summary);
    }
}
