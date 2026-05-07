using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers
{
  [Authorize]
  public class ReportsController(ReportLogRepository repo, ReportService reportService) : Controller
  {
    public async Task<IActionResult> Logs()
    {
      var logs = await repo.GetAll();
      return View(logs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendNow()
    {
      var sent = await reportService.SendAllReports();
      TempData["Success"] = $"Đã gửi {sent} báo cáo.";
      return RedirectToAction(nameof(Logs));
    }
  }
}
