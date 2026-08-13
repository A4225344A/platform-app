using PlatformService.Services;

namespace PlatformService.Tests.Unit;

public class WeatherForecastServiceTests
{
    private readonly IWeatherForecastService _service = new WeatherForecastService();

    [Fact]
    public void GetForecast_ReturnsFiveDaysWithSummaries()
    {
        var forecast = _service.GetForecast();

        Assert.Equal(5, forecast.Length);
        Assert.All(forecast, f => Assert.False(string.IsNullOrWhiteSpace(f.Summary)));
    }

    [Fact]
    public void GetForecast_IncrementsBusinessCounter()
    {
        var before = BusinessMetrics.RequestsProcessed.Value;

        _service.GetForecast();

        Assert.Equal(before + 1, BusinessMetrics.RequestsProcessed.Value);
    }
}
