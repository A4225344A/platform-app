using Microsoft.AspNetCore.Mvc;
using PlatformService.Models;
using PlatformService.Services;

namespace PlatformService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;

    public WeatherForecastController(IWeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(WeatherForecast[]), StatusCodes.Status200OK)]
    public ActionResult<WeatherForecast[]> Get()
    {
        return Ok(_weatherForecastService.GetForecast());
    }
}
