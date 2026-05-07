using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers
{
  [Authorize]
  public class LiveConfigsController(
    LiveConfigRepository repo,
    LiveAdRepository liveAdRepo,
    AuditRepository auditRepo
  ) : Controller
  {
    public async Task<IActionResult> Index()
    {
      var data = await repo.GetAll();

      return View(data);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(LiveConfig model)
    {
      var id = await repo.Create(model);

      await auditRepo.Create(
        "CREATE",
        "live_configs",
        id,
        null,
        model,
        User.Identity?.Name
      );

      return RedirectToAction(nameof(Index));
    }

    public IActionResult AddAd(long id)
    {
      var model = new LiveAd
      {
        LiveConfigId = id
      };

      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddAd(
      LiveAd model)
    {
      await liveAdRepo.Create(model);

      return RedirectToAction(nameof(Index));
    }
  }
}
