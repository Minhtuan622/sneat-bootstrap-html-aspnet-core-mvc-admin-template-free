using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

[Authorize]
public class DashboardsController(LiveMetricsRepository repo, Microsoft.Extensions.Caching.Memory.IMemoryCache cache) : Controller
{
  public IActionResult Index() => View();

  public async Task<IActionResult> Live([FromQuery] AspnetCoreMvcFull.Models.DashboardFilterModel filter)
  {
    var cacheKey = $"live_metrics:{filter.ProjectName}:{filter.PostId}:{filter.FromDate}:{filter.ToDate}:{filter.Status}";
    var data = await cache.GetOrCreateAsync(cacheKey, async e => { e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30); return await repo.GetMetrics(filter); }) ?? [];
    ViewBag.Filter = filter;
    return View(data);
  }
}
