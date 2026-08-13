using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PlatformService.Models;

namespace PlatformService.Tests.Integration;

public class WeatherForecastEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WeatherForecastEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task WeatherForecast_ReturnsFiveDays()
    {
        var response = await _client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();

        var forecast = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();

        Assert.NotNull(forecast);
        Assert.Equal(5, forecast!.Length);
        Assert.All(forecast, f => Assert.False(string.IsNullOrWhiteSpace(f.Summary)));
    }
}
