using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace AspnetCoreMvcFull.Controllers;

[Authorize]
public class HealthController(
  IConfiguration config,
  SystemSettingsRepository settings,
  IMemoryCache cache)
  : Controller
{
  public async Task<IActionResult> Index()
  {
    var items = new List<(string Service, string Status)>
    {
      (
        "SQL Server",
        await CheckSql(
          config.GetConnectionString("DefaultConnection")!
        )
          ? "OK"
          : "FAIL"
      ),

      (
        "Lark Webhook",
        CheckLark(config)
          ? "OK"
          : "FAIL"
      )
    };

    // Worker enable/disable status
    var enableWorker = await settings.GetValue("enable_worker");

    items.Add(
      (
        "Worker Config",
        string.Equals(
          enableWorker,
          "false",
          StringComparison.OrdinalIgnoreCase
        )
          ? "Stopped"
          : "Running"
      )
    );

    // Last report time
    var lastReport =
      cache.TryGetValue<DateTime>(
        "health_last_report",
        out var reportTs
      )
        ? $"{(int)(DateTime.UtcNow - reportTs).TotalMinutes} phút trước"
        : "N/A";

    // Last worker execution time
    var lastWorkerRun =
      cache.TryGetValue<DateTime>(
        "worker_last_run",
        out var workerTs
      )
        ? $"{(int)(DateTime.UtcNow - workerTs).TotalMinutes} phút trước"
        : "N/A";

    // Worker runtime status
    var workerStatus = "Running";

    if (
      workerTs != default &&
      (DateTime.UtcNow - workerTs).TotalMinutes > 10
    )
    {
      workerStatus = "STALE";
    }

    items.Add(("Worker Health", workerStatus));

    ViewBag.LastReport = lastReport;
    ViewBag.LastWorkerRun = lastWorkerRun;

    return View(items);
  }

  private static async Task<bool> CheckSql(string connectionString)
  {
    try
    {
      await using var connection = new SqlConnection(connectionString);

      await connection.OpenAsync();

      return true;
    }
    catch
    {
      return false;
    }
  }

  private static bool CheckLark(IConfiguration config)
  {
    return !string.IsNullOrWhiteSpace(
      config["Lark:Webhook"]
    );
  }
}
