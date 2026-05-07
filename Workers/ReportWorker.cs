using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;

namespace AspnetCoreMvcFull.Workers
{
  public class ReportWorker(IServiceProvider serviceProvider) : BackgroundService
  {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        using var scope = serviceProvider.CreateScope();
        try
        {
          var settings = scope.ServiceProvider.GetRequiredService<SystemSettingsRepository>();
          var dispatcher = scope.ServiceProvider.GetRequiredService<ReportService>();

          var enableWorker = await settings.GetValue("enable_worker");
          if (!string.Equals(enableWorker, "false", StringComparison.OrdinalIgnoreCase))
          {
            await dispatcher.SendAllReports();
          }

          var intervalText = await settings.GetValue("report_interval_minutes");
          var interval = int.TryParse(intervalText, out var m) ? m : 5;
          await Task.Delay(TimeSpan.FromMinutes(interval), stoppingToken);
        }
        catch (Exception ex)
        {
          var errorLogService = scope.ServiceProvider.GetRequiredService<ErrorLogService>();
          await errorLogService.Log(ex, "ReportWorker.ExecuteAsync");
          await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
      }
    }
  }
}
