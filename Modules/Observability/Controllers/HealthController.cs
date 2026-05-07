using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AspnetCoreMvcFull.Controllers;

[Authorize(Roles = "Admin")]
public class HealthController(IConfiguration config, LarkService larkService, SystemSettingsRepository settings, IMemoryCache cache) : Controller
{
  public async Task<IActionResult> Index()
  {
    var items = new List<(string Service, string Status)>();
    items.Add(("SQL Server", await CheckSql(config.GetConnectionString("DefaultConnection")!) ? "OK" : "FAIL"));
    items.Add(("Lark Webhook", await CheckLark(larkService) ? "OK" : "FAIL"));
    var enableWorker = await settings.GetValue("enable_worker");
    items.Add(("Worker", string.Equals(enableWorker, "false", StringComparison.OrdinalIgnoreCase) ? "Stopped" : "Running"));
    var lastReport = cache.TryGetValue<DateTime>("health_last_report", out var ts) ? $"{(int)(DateTime.UtcNow-ts).TotalMinutes} phút trước" : "N/A";
    ViewBag.LastReport = lastReport;
    return View(items);
  }

  private static async Task<bool> CheckSql(string conn)
  {
    try { await using var c = new Microsoft.Data.SqlClient.SqlConnection(conn); await c.OpenAsync(); return true; } catch { return false; }
  }

  private static async Task<bool> CheckLark(LarkService lark)
  {
    try { await lark.Send("[health-check] ping"); return true; } catch { return false; }
  }
}
