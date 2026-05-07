using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

[Authorize(Roles = "Admin")]
public class SystemSettingsController(SystemSettingsRepository repo) : Controller
{
  public async Task<IActionResult> Index()
  {
    var settings = await repo.GetAll();
    return View(settings);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Save(string key, string value)
  {
    await repo.Upsert(key, value);
    TempData["Success"] = $"Đã lưu {key}";
    return RedirectToAction(nameof(Index));
  }
}
