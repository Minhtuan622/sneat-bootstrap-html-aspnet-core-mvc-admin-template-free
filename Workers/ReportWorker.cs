using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;

namespace AspnetCoreMvcFull.Workers
{
  public class ReportWorker(
    IServiceProvider serviceProvider,
    ILogger<ReportWorker> logger
  ) : BackgroundService
  {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          using var scope = serviceProvider.CreateScope();
          var settings = scope.ServiceProvider.GetRequiredService<SystemSettingsRepository>();
          var dispatcher = scope.ServiceProvider.GetRequiredService<ReportDispatchService>();

          var enableWorker = await settings.GetValue("enable_worker");
          if (!string.Equals(enableWorker, "false", StringComparison.OrdinalIgnoreCase))
          {
            await dispatcher.DispatchChangedReports();
          }

          var intervalText = await settings.GetValue("report_interval_minutes");
          var interval = int.TryParse(intervalText, out var m) ? m : 5;
          await Task.Delay(TimeSpan.FromMinutes(interval), stoppingToken);
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "WORKER ERROR");
          await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
      }
    }
  }
}
