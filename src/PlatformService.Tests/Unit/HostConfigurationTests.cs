namespace PlatformService.Tests.Unit;

public class HostConfigurationTests
{
    [Fact]
    public void DefaultListenUrl_IsAllInterfacesPort8080()
    {
        Assert.Equal("http://0.0.0.0:8080", PlatformServiceApp.DefaultListenUrl);
    }

    [Fact]
    public void ShutdownTimeout_Is30Seconds()
    {
        Assert.Equal(TimeSpan.FromSeconds(30), PlatformServiceApp.ShutdownTimeout);
    }
}
