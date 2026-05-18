using AspnetCoreMvcFull.Constants;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AspnetCoreMvcFull.Controllers;

[Authorize(Roles = Roles.Admin)]
public class DashboardsController(
  LiveMetricsRepository repo,
  LiveMetricSnapshotRepository snapshotRepo,
  IMemoryCache cache
) : Controller
{
  public IActionResult Index() => View();

  public async Task<IActionResult> Live([FromQuery] DashboardFilterModel filter)
  {
    var cacheKey =
      $"live_metrics:{filter.ProjectName}:{filter.PostId}:{filter.FromDate}:{filter.ToDate}:{filter.Status}";
    var data = await cache.GetOrCreateAsync(cacheKey, async e =>
    {
      e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
      return await repo.GetMetrics(filter);
    }) ?? [];
    ViewBag.Filter = filter;
    return View(data);
  }

  public async Task<IActionResult> Trend(long id)
  {
    var data = await snapshotRepo.GetByConfig(id);

    return Json(data);
  }
}
