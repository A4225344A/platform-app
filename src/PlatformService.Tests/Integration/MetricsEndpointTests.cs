using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PlatformService.Tests.Integration;

public class MetricsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MetricsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Metrics_ReturnsPrometheusTextFormat()
    {
        var response = await _client.GetAsync("/metrics");

        response.EnsureSuccessStatusCode();
        Assert.StartsWith("text/plain", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Metrics_ExposesCustomBusinessCounter()
    {
        var body = await (await _client.GetAsync("/metrics")).Content.ReadAsStringAsync();

        Assert.Contains("platformservice_requests_processed_total", body);
    }

    [Fact]
    public async Task Metrics_CounterIncrementsWhenBusinessEndpointIsHit()
    {
        var before = await ReadCounterValueAsync();

        await _client.GetAsync("/weatherforecast");
        await _client.GetAsync("/weatherforecast");
        await _client.GetAsync("/weatherforecast");

        var after = await ReadCounterValueAsync();

        Assert.Equal(before + 3, after);
    }

    private async Task<double> ReadCounterValueAsync()
    {
        var body = await (await _client.GetAsync("/metrics")).Content.ReadAsStringAsync();
        var match = Regex.Match(body, @"^platformservice_requests_processed_total\s+(\S+)$", RegexOptions.Multiline);

        Assert.True(match.Success, "找不到 platformservice_requests_processed_total 指標");
        return double.Parse(match.Groups[1].Value);
    }
}
