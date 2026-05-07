using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

public class PagesController(UserRepository userRepository, IWebHostEnvironment env) : Controller
{
  public IActionResult AccountSettings() => View();
  public IActionResult AccountSettingsConnections() => View();
  public IActionResult AccountSettingsNotifications() => View();
  public IActionResult MiscError() => View();
  public IActionResult MiscUnderMaintenance() => View();

  public async Task<IActionResult> ProfileUser()
  {
    var username = User.Identity?.Name;
    if (string.IsNullOrWhiteSpace(username)) return RedirectToAction("LoginBasic", "Auth");

    var user = await userRepository.GetByUsername(username);
    if (user == null) return NotFound();

    return View(new ProfileUserViewModel
    {
      Username = user.Username,
      FullName = user.FullName,
      RoleName = user.RoleName,
      AvatarPath = user.AvatarPath
    });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> ProfileUser(ProfileUserViewModel model, IFormFile? avatar)
  {
    var username = User.Identity?.Name;
    if (string.IsNullOrWhiteSpace(username)) return RedirectToAction("LoginBasic", "Auth");
    if (!ModelState.IsValid) return View(model);

    var user = await userRepository.GetByUsername(username);
    if (user == null) return NotFound();

    var avatarPath = user.AvatarPath;
    if (avatar != null && avatar.Length > 0)
    {
      var ext = Path.GetExtension(avatar.FileName).ToLowerInvariant();
      var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
      if (!allowed.Contains(ext))
      {
        ModelState.AddModelError("AvatarPath", "Chỉ hỗ trợ JPG/PNG/WEBP");
        model.AvatarPath = avatarPath;
        return View(model);
      }

      var folder = Path.Combine(env.WebRootPath, "uploads", "avatars");
      Directory.CreateDirectory(folder);
      var fileName = $"{username}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
      var filePath = Path.Combine(folder, fileName);
      await using var fs = new FileStream(filePath, FileMode.Create);
      await avatar.CopyToAsync(fs);
      avatarPath = $"/uploads/avatars/{fileName}";
    }

    await userRepository.UpdateProfile(username, model.FullName, avatarPath);

    TempData["Success"] = "Cập nhật profile thành công. Vui lòng đăng nhập lại để cập nhật thông tin trên navbar.";
    return RedirectToAction(nameof(ProfileUser));
  }
}
