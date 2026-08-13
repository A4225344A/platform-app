using System.Text.Json;

namespace PlatformService.Tests.Unit;

public class LoggingTests
{
    [Fact]
    public void CreateLogger_WritesSingleLineJson()
    {
        using var writer = new StringWriter();
        var logger = PlatformServiceLogging.CreateLogger("test-service", writer);
        logger.Information("hello {Name}", "world");

        var output = writer.ToString();
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        Assert.Single(lines);
    }

    [Fact]
    public void CreateLogger_OutputIsValidJson()
    {
        using var writer = new StringWriter();
        var logger = PlatformServiceLogging.CreateLogger("test-service", writer);
        logger.Information("hello {Name}", "world");

        var line = writer.ToString().TrimEnd('\n');
        using var doc = JsonDocument.Parse(line);

        Assert.Equal("hello {Name}", doc.RootElement.GetProperty("@mt").GetString());
        Assert.Equal("world", doc.RootElement.GetProperty("Name").GetString());
    }

    [Theory]
    [InlineData("platform-service")]
    [InlineData("another-service-name")]
    public void CreateLogger_IncludesServiceFieldFromGivenName(string serviceName)
    {
        using var writer = new StringWriter();
        var logger = PlatformServiceLogging.CreateLogger(serviceName, writer);
        logger.Information("ping");

        var line = writer.ToString().TrimEnd('\n');
        using var doc = JsonDocument.Parse(line);

        Assert.Equal(serviceName, doc.RootElement.GetProperty("service").GetString());
    }
}
