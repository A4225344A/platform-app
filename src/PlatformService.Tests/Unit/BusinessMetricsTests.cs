using PlatformService.Services;

namespace PlatformService.Tests.Unit;

public class BusinessMetricsTests
{
    [Fact]
    public void RequestsProcessed_HasExpectedNameAndHelp()
    {
        Assert.Equal("platformservice_requests_processed_total", BusinessMetrics.RequestsProcessed.Name);
        Assert.Equal("已處理的業務請求總數", BusinessMetrics.RequestsProcessed.Help);
    }

    [Fact]
    public void RequestsProcessed_IncrementsByOneEachCall()
    {
        var before = BusinessMetrics.RequestsProcessed.Value;

        BusinessMetrics.RequestsProcessed.Inc();
        BusinessMetrics.RequestsProcessed.Inc();

        Assert.Equal(before + 2, BusinessMetrics.RequestsProcessed.Value);
    }
}
