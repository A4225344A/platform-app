using Prometheus;

namespace PlatformService.Services;

public static class BusinessMetrics
{
    public static readonly Counter RequestsProcessed = Metrics.CreateCounter(
        "platformservice_requests_processed_total",
        "已處理的業務請求總數"); 
}
