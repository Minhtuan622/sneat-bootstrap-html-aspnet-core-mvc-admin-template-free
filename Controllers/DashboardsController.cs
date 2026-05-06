using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

[Authorize]
public class DashboardsController(LiveMetricsRepository repo) : Controller
{
  public IActionResult Index() => View();

  public async Task<IActionResult> Live()
  {
    var data = await repo.GetMetrics();

    return View(data);
  }
}
