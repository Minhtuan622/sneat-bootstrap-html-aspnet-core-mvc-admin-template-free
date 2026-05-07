using System.Security.Claims;
using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

public class AuthController(UserRepository repo) : Controller
{
  [AllowAnonymous]
  public IActionResult ForgotPasswordBasic() => View();

  [AllowAnonymous]
  public IActionResult LoginBasic() => View();

  [AllowAnonymous]
  public IActionResult RegisterBasic() => View();

  [HttpPost]
  [AllowAnonymous]
  public async Task<IActionResult> LoginBasic(string username, string password)
  {
    var user = await repo.GetByUsername(username);

    if (user == null)
    {
      ViewBag.Error = "Sai tài khoản";
      return View();
    }

    if (!user.IsActive)
    {
      ViewBag.Error = "Tài khoản đã bị khóa";
      return View();
    }

    if (!PasswordHasher.Verify(password, user.PasswordHash) && user.PasswordHash != password)
    {
      ViewBag.Error = "Sai mật khẩu";
      return View();
    }

    var claims = new List<Claim>
    {
      new(ClaimTypes.Name, user.Username),
      new(ClaimTypes.Role, user.RoleName),
      new("full_name", user.FullName),
      new("avatar_path", user.AvatarPath ?? string.Empty)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return RedirectToAction("Index", "Dashboards");
  }

  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    return RedirectToAction(nameof(LoginBasic));
  }
}
