using PlatformService;
using Serilog;

var serviceName = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "platform-service";

Log.Logger = PlatformServiceLogging.CreateLogger(serviceName);

try
{
    var app = PlatformServiceApp.Build(args);
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

// 讓 WebApplicationFactory<Program> 能在測試專案中啟動這個進入點
public partial class Program { }
