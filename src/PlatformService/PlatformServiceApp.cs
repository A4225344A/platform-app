using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using PlatformService.Services;
using Prometheus;
using Serilog;

namespace PlatformService;

public static class PlatformServiceApp
{
    public const string DefaultListenUrl = "http://0.0.0.0:8080";
    public static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(30);

    public static WebApplication Build(string[] args, string? listenUrl = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();

        // 監聽 0.0.0.0:8080(測試可覆寫成動態連接埠,避免搶用真正的 8080)
        builder.WebHost.UseUrls(listenUrl ?? DefaultListenUrl);

        // 優雅關閉:SIGTERM 進來後,等待進行中的請求處理完再退出
        builder.Host.ConfigureHostOptions(options =>
        {
            options.ShutdownTimeout = ShutdownTimeout;
        });

        // Controller 層
        builder.Services.AddControllers();

        // Swagger:掃描 Controller 上的屬性/路由,產生可互動測試的 API 文件
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PlatformService API",
                Version = "v1",
            });
        });

        // liveness 只確認程式活著,不掛任何檢查;readiness 用 tag 篩選要確認能接流量的檢查
        builder.Services.AddPlatformHealthChecks();

        // Service 層:注入到 Controller
        builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService API v1");
            });
        }

        // prometheus-net:記錄每個 HTTP 請求的內建指標(耗時、狀態碼等)
        app.UseHttpMetrics();

        // 啟動時就把自訂計數器註冊到 registry(初始值 0),避免要等第一次呼叫業務端點
        // 才出現在 /metrics,讓 rate() 這類查詢從程式啟動起就能正確運作。
        _ = BusinessMetrics.RequestsProcessed;

        // liveness:只回應程式是否還活著,不執行任何相依性檢查
        app.MapHealthChecks("/healthz", new HealthCheckOptions
        {
            Predicate = _ => false,
        });

        // readiness:執行所有標記為 "ready" 的檢查,確認能接流量
        app.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(HealthCheckSetup.ReadyTag),
        });

        // /metrics:暴露 prometheus-net 指標(含自訂業務計數器)
        app.MapMetrics("/metrics");

        // 業務 API:走 Controller -> Service -> Model 三層架構
        app.MapControllers();

        return app;
    }
}

public static class HealthCheckSetup
{
    public const string ReadyTag = "ready";

    public static IHealthChecksBuilder AddPlatformHealthChecks(this IServiceCollection services)
        => services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { ReadyTag });
}
