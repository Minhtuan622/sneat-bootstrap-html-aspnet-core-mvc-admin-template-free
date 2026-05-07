using AspnetCoreMvcFull.Services;

namespace AspnetCoreMvcFull.Services;

public class ReportDispatchService(ReportService reportService)
{
  public Task<int> DispatchChangedReports() => reportService.SendAllReports();
}
