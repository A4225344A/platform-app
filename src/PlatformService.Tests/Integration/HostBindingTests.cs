using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace PlatformService.Tests.Integration;

/// <summary>
/// 這裡用真正的 Kestrel(而非 WebApplicationFactory 的 TestServer)驗證監聽位址與優雅關閉設定,
/// 連接埠用 0 讓作業系統動態配置,避免佔用正式的 8080 或造成測試間衝突。
/// </summary>
public class HostBindingTests
{
    [Fact]
    public async Task Build_BindsToAllInterfaces()
    {
        var app = PlatformServiceApp.Build(Array.Empty<string>(), listenUrl: "http://0.0.0.0:0");

        await app.StartAsync();
        try
        {
            var addressesFeature = app.Services
                .GetRequiredService<IServer>()
                .Features
                .Get<IServerAddressesFeature>();

            Assert.NotNull(addressesFeature);
            Assert.Contains(addressesFeature!.Addresses, address => address.Contains("0.0.0.0"));
        }
        finally
        {
            await app.StopAsync();
        }
    }

    [Fact]
    public async Task Build_ConfiguresShutdownTimeoutTo30Seconds()
    {
        await using var app = PlatformServiceApp.Build(Array.Empty<string>(), listenUrl: "http://127.0.0.1:0");

        var hostOptions = app.Services.GetRequiredService<IOptions<HostOptions>>().Value;

        Assert.Equal(TimeSpan.FromSeconds(30), hostOptions.ShutdownTimeout);
    }

    [Fact]
    public async Task Build_StartsAndStopsGracefullyWithinShutdownTimeout()
    {
        var app = PlatformServiceApp.Build(Array.Empty<string>(), listenUrl: "http://127.0.0.1:0");
        await app.StartAsync();

        var stopwatch = Stopwatch.StartNew();
        await app.StopAsync(TimeSpan.FromSeconds(35));
        stopwatch.Stop();

        // 沒有進行中的請求時應該很快就關閉,遠低於 30 秒的優雅關閉逾時上限
        Assert.True(stopwatch.Elapsed < PlatformServiceApp.ShutdownTimeout);
    }
}
