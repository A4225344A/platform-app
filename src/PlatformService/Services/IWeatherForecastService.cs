using PlatformService.Models;

namespace PlatformService.Services;

public interface IWeatherForecastService
{
    WeatherForecast[] GetForecast();
}
