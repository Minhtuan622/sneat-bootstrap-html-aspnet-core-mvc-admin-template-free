using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

[Authorize]
public class DashboardsController(LiveMetricsRepository repo) : Controller
{
  public IActionResult Index() => View();

  public async Task<IActionResult> Live([FromQuery] AspnetCoreMvcFull.Models.DashboardFilterModel filter)
  {
    var data = await repo.GetMetrics(filter);
    ViewBag.Filter = filter;
    return View(data);
  }
}
