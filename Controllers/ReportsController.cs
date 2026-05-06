using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers
{
  [Authorize]
  public class ReportsController(ReportLogRepository repo) : Controller
  {
    public async Task<IActionResult> Logs()
    {
      var logs = await repo.GetAll();

      return View(logs);
    }
  }
}
