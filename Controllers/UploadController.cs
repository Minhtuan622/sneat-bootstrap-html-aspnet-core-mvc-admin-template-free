using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers
{
  [Authorize]
  public class UploadController(ImportJobRepository jobRepo, IWebHostEnvironment env) : Controller
  {
    public async Task<IActionResult> Orders()
    {
      ViewBag.Jobs = await jobRepo.GetRecent();
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Orders(IFormFile? file)
    {
      if (file == null || file.Length == 0)
      {
        ViewBag.Error = "File không hợp lệ";
        ViewBag.Jobs = await jobRepo.GetRecent();
        return View();
      }

      var folder = Path.Combine(env.WebRootPath, "uploads", "imports");
      Directory.CreateDirectory(folder);
      var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Path.GetFileName(file.FileName)}";
      var fullPath = Path.Combine(folder, fileName);
      await using var fs = new FileStream(fullPath, FileMode.Create);
      await file.CopyToAsync(fs);

      var jobId = await jobRepo.Create(fullPath, file.FileName, User.Identity?.Name ?? "system");
      ViewBag.Success = $"Đã queue import job #{jobId}.";
      ViewBag.Jobs = await jobRepo.GetRecent();
      return View();
    }
  }
}
