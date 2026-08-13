using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PlatformService.Tests.Unit;

public class HealthCheckSetupTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPlatformHealthChecks();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ReadyTaggedCheck_ReportsHealthy()
    {
        await using var provider = BuildProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();

        var report = await healthCheckService.CheckHealthAsync(
            registration => registration.Tags.Contains(HealthCheckSetup.ReadyTag));

        Assert.Equal(HealthStatus.Healthy, report.Status);
        Assert.Contains("self", report.Entries.Keys);
    }

    [Fact]
    public async Task LivenessPredicate_ExcludingEverything_RunsNoChecks()
    {
        await using var provider = BuildProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();

        // 對應 /healthz 的 Predicate = _ => false
        var report = await healthCheckService.CheckHealthAsync(_ => false);

        Assert.Empty(report.Entries);
        Assert.Equal(HealthStatus.Healthy, report.Status);
    }
}
