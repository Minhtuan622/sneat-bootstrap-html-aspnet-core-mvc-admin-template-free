using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;

namespace AspnetCoreMvcFull.Services;

public class ReportDispatchService(
  LiveMetricsRepository repo,
  ReportBuilderService builder,
  LarkService lark,
  ReportLogRepository reportLogRepo,
  LiveMetricSnapshotRepository snapshotRepo)
{
  public async Task<int> DispatchChangedReports()
  {
    var sent = 0;
    var metrics = await repo.GetMetrics();

    foreach (var item in metrics)
    {
      var message = builder.Build(item);
      try
      {
        var latest = await snapshotRepo.GetLatest(item.Id);
        var changed = latest == null || latest.TotalRevenue != item.TotalRevenue || latest.TotalSpend != item.TotalSpend || latest.Profit != item.EstimatedProfit;
        if (!changed) continue;

        await lark.Send(message);
        sent++;

        await reportLogRepo.Create(new ReportLog { LiveConfigId = item.Id, Message = message, IsSuccess = true });
        await snapshotRepo.Create(new LiveMetricSnapshot { LiveConfigId = item.Id, TotalRevenue = item.TotalRevenue, TotalSpend = item.TotalSpend, Profit = item.EstimatedProfit });
      }
      catch (Exception ex)
      {
        await reportLogRepo.Create(new ReportLog { LiveConfigId = item.Id, Message = message, IsSuccess = false, ErrorMessage = ex.Message });
      }
    }

    return sent;
  }
}
