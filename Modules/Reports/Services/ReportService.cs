using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace AspnetCoreMvcFull.Services;

public class ReportService(
  LiveMetricsRepository repo,
  ReportBuilderService builder,
  LarkService lark,
  ReportLogRepository reportLogRepo,
  LiveMetricSnapshotRepository snapshotRepo,
  IMemoryCache cache,
  ErrorLogService errorLogService)
{
  public async Task<int> SendAllReports()
  {
    var sent = 0;
    var metrics = await GetMetricsCached();
    foreach (var item in metrics)
    {
      if (await SendReport(item)) sent++;
    }
    return sent;
  }

  public async Task<bool> SendReport(long liveConfigId)
  {
    var metric = (await GetMetricsCached()).FirstOrDefault(x => x.Id == liveConfigId);
    if (metric == null) return false;
    return await SendReport(metric);
  }

  private async Task<IEnumerable<LiveMetric>> GetMetricsCached()
  {
    return await cache.GetOrCreateAsync("report_metrics", async e =>
    {
      e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(45);
      return await repo.GetMetrics();
    }) ?? [];
  }

  private async Task<bool> SendReport(LiveMetric item)
  {
    var message = builder.Build(item);
    try
    {
      var latest = await snapshotRepo.GetLatest(item.Id);
      var changed = latest == null || latest.TotalRevenue != item.TotalRevenue || latest.TotalSpend != item.TotalSpend || latest.Profit != item.EstimatedProfit;
      if (!changed) return false;

      await lark.Send(message);
      await reportLogRepo.Create(new ReportLog { LiveConfigId = item.Id, Message = message, IsSuccess = true });
      await snapshotRepo.Create(new LiveMetricSnapshot { LiveConfigId = item.Id, TotalRevenue = item.TotalRevenue, TotalSpend = item.TotalSpend, Profit = item.EstimatedProfit });
      cache.Set("health_last_report", DateTime.UtcNow, TimeSpan.FromHours(2));
      return true;
    }
    catch (Exception ex)
    {
      await reportLogRepo.Create(new ReportLog { LiveConfigId = item.Id, Message = message, IsSuccess = false, ErrorMessage = ex.Message });
      await errorLogService.Log(ex, "ReportService.SendReport");
      return false;
    }
  }
}
