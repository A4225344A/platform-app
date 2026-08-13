using PlatformService.Models;

namespace PlatformService.Tests.Unit;

public class WeatherForecastTests
{
    [Theory]
    [InlineData(0, 32)]
    [InlineData(100, 211)]
    [InlineData(-40, -39)]
    [InlineData(20, 67)]
    public void TemperatureF_ConvertsFromCelsius(int celsius, int expectedFahrenheit)
    {
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), celsius, "Test");

        Assert.Equal(expectedFahrenheit, forecast.TemperatureF);
    }
}
