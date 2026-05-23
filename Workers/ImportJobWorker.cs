using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;
using OfficeOpenXml;

namespace AspnetCoreMvcFull.Workers;

public class ImportJobWorker(IServiceProvider serviceProvider) : BackgroundService
{
  [Obsolete("Obsolete")]
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
        using var scope = serviceProvider.CreateScope();
      try
      {
        var repo = scope.ServiceProvider.GetRequiredService<ImportJobRepository>();
        var importer = scope.ServiceProvider.GetRequiredService<ExcelImportService>();

        var job = await repo.GetQueuedJob();
        if (job == null)
        {
          await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
          continue;
        }

        using var package = new ExcelPackage(new FileInfo(job.FilePath));
        var totalRows = Math.Max(0, (package.Workbook.Worksheets[0].Dimension?.Rows ?? 1) - 1);
        await repo.Start(job.Id, totalRows);

        var result = await importer.ImportOrdersFromPath(job.FilePath);
        await repo.Complete(job.Id, result.SuccessCount, result.FailedCount);
      }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
      {
        // app đang shutdown bình thường, bỏ qua
        return;
      }
      catch (Exception ex)
      {
        var errors = scope.ServiceProvider.GetRequiredService<ErrorLogService>();
        await errors.Log(ex, "ImportJobWorker Error");
      }
    }
  }
}
