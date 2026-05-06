using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers
{
  [Authorize]
  public class UploadController(ExcelImportService excel) : Controller
  {
    public IActionResult Orders()
    {
      return View();
    }

    [HttpPost]
    [Obsolete("Obsolete")]
    public async Task<IActionResult> Orders(IFormFile? file)
    {
      if (file == null || file.Length == 0)
      {
        ViewBag.Error = "File không hợp lệ";

        return View();
      }

      var result = await excel.ImportOrders(file);

      ViewBag.Result = result;

      return View();
    }
  }
}
