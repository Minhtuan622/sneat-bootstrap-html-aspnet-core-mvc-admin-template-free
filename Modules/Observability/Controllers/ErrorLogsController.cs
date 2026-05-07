using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

[Authorize(Roles = "Admin")]
public class ErrorLogsController(ErrorLogRepository repo) : Controller
{
  public async Task<IActionResult> Index() => View(await repo.GetAll());
}
