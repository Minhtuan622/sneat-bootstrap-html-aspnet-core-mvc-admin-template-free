using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

[Authorize(Roles = UserRole.Admin)]
public class UsersController(UserRepository userRepository) : Controller
{
  public async Task<IActionResult> Index()
  {
    var users = await userRepository.GetAll();
    return View(users);
  }

  public IActionResult Create()
  {
    return View("Form", new UserFormModel());
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(UserFormModel model)
  {
    if (string.IsNullOrWhiteSpace(model.Password))
      ModelState.AddModelError(nameof(model.Password), "Mật khẩu là bắt buộc");

    if (await userRepository.ExistsUsername(model.Username))
      ModelState.AddModelError(nameof(model.Username), "Username đã tồn tại");

    if (!ModelState.IsValid) return View("Form", model);

    await userRepository.Create(model, PasswordHasher.Hash(model.Password!));
    TempData["Success"] = "Tạo người dùng thành công";
    return RedirectToAction(nameof(Index));
  }

  public async Task<IActionResult> Edit(long id)
  {
    var user = await userRepository.GetById(id);
    if (user == null) return NotFound();

    return View("Form", new UserFormModel
    {
      Id = user.Id,
      Username = user.Username,
      FullName = user.FullName,
      RoleName = user.RoleName,
      IsActive = user.IsActive
    });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(UserFormModel model)
  {
    if (model.Id == null) return BadRequest();

    if (await userRepository.ExistsUsername(model.Username, model.Id))
      ModelState.AddModelError(nameof(model.Username), "Username đã tồn tại");

    if (!ModelState.IsValid) return View("Form", model);

    await userRepository.Update(model);

    if (!string.IsNullOrWhiteSpace(model.Password))
      await userRepository.ResetPassword(model.Id.Value, PasswordHasher.Hash(model.Password));

    TempData["Success"] = "Cập nhật người dùng thành công";
    return RedirectToAction(nameof(Index));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(long id)
  {
    await userRepository.Delete(id);
    TempData["Success"] = "Xóa người dùng thành công";
    return RedirectToAction(nameof(Index));
  }
}
